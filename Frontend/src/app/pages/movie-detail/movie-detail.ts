import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, input, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { DurationPipe } from '../../core/duration.pipe';
import { describeError } from '../../core/http-error';
import { Movie } from '../../core/movie.model';
import { MovieService } from '../../core/movie.service';

@Component({
  selector: 'app-movie-detail',
  imports: [RouterLink, DatePipe, DurationPipe],
  templateUrl: './movie-detail.html',
  styleUrl: './movie-detail.scss',
})
export class MovieDetailComponent implements OnInit {
  private readonly api = inject(MovieService);
  private readonly router = inject(Router);

  /** Bound from the route param ":id". */
  readonly id = input.required<string>();

  protected readonly movie = signal<Movie | null>(null);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly confirming = signal(false);
  protected readonly deleting = signal(false);

  ngOnInit(): void {
    this.api.getById(Number(this.id())).subscribe({
      next: (movie) => {
        this.movie.set(movie);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(describeError(err));
        this.loading.set(false);
      },
    });
  }

  protected delete(): void {
    const movie = this.movie();
    if (!movie) return;

    this.deleting.set(true);
    this.api.delete(movie.movieId).subscribe({
      next: () => this.router.navigate(['/movies']),
      error: (err) => {
        this.error.set(describeError(err));
        this.deleting.set(false);
        this.confirming.set(false);
      },
    });
  }
}
