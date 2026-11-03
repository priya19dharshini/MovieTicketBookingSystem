import { DatePipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { catchError, debounceTime, distinctUntilChanged, of, startWith, switchMap, tap } from 'rxjs';

import { DurationPipe } from '../../core/duration.pipe';
import { describeError } from '../../core/http-error';
import { Movie } from '../../core/movie.model';
import { MovieService } from '../../core/movie.service';

@Component({
  selector: 'app-movie-list',
  imports: [RouterLink, ReactiveFormsModule, DatePipe, DurationPipe],
  templateUrl: './movie-list.html',
  styleUrl: './movie-list.scss',
})
export class MovieListComponent {
  private readonly api = inject(MovieService);

  protected readonly movies = signal<Movie[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);

  protected readonly searchControl = new FormControl('', { nonNullable: true });
  protected readonly selectedGenre = signal('');

  /** Movie currently awaiting delete confirmation. */
  protected readonly pendingDelete = signal<Movie | null>(null);
  protected readonly deleting = signal(false);

  protected readonly genres = computed(() =>
    [...new Set(this.movies().map((m) => m.genre))].sort((a, b) => a.localeCompare(b)),
  );

  protected readonly visibleMovies = computed(() => {
    const genre = this.selectedGenre();
    return genre ? this.movies().filter((m) => m.genre === genre) : this.movies();
  });

  constructor() {
    // Empty box -> GET /api/movies ; otherwise GET /api/movies/search?title=...
    this.searchControl.valueChanges
      .pipe(
        startWith(this.searchControl.value),
        debounceTime(300),
        distinctUntilChanged(),
        tap(() => {
          this.loading.set(true);
          this.error.set(null);
        }),
        switchMap((term) => {
          const title = term.trim();
          const request$ = title ? this.api.search(title) : this.api.getAll();
          return request$.pipe(
            catchError((err) => {
              this.error.set(describeError(err));
              return of<Movie[]>([]);
            }),
          );
        }),
        takeUntilDestroyed(),
      )
      .subscribe((movies) => {
        this.movies.set(movies);
        if (this.selectedGenre() && !movies.some((m) => m.genre === this.selectedGenre())) {
          this.selectedGenre.set('');
        }
        this.loading.set(false);
      });
  }

  protected onGenreChange(event: Event): void {
    this.selectedGenre.set((event.target as HTMLSelectElement).value);
  }

  protected clearFilters(): void {
    this.searchControl.setValue('');
    this.selectedGenre.set('');
  }

  protected askDelete(movie: Movie): void {
    this.pendingDelete.set(movie);
  }

  protected cancelDelete(): void {
    if (!this.deleting()) this.pendingDelete.set(null);
  }

  protected confirmDelete(): void {
    const movie = this.pendingDelete();
    if (!movie) return;

    this.deleting.set(true);
    this.api.delete(movie.movieId).subscribe({
      next: () => {
        this.movies.update((list) => list.filter((m) => m.movieId !== movie.movieId));
        this.deleting.set(false);
        this.pendingDelete.set(null);
      },
      error: (err) => {
        this.error.set(describeError(err));
        this.deleting.set(false);
        this.pendingDelete.set(null);
      },
    });
  }
}
