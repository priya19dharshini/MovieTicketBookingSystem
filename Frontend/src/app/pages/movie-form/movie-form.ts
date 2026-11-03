import { Component, OnInit, inject, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { describeError } from '../../core/http-error';
import { MovieRequest } from '../../core/movie.model';
import { MovieService } from '../../core/movie.service';

type FieldName = 'title' | 'description' | 'durationMinutes' | 'releaseDate' | 'language' | 'genre';

@Component({
  selector: 'app-movie-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './movie-form.html',
  styleUrl: './movie-form.scss',
})
export class MovieFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(MovieService);
  private readonly router = inject(Router);

  /** Bound from the route param ":id" (withComponentInputBinding). Undefined when adding. */
  readonly id = input<string>();

  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly error = signal<string | null>(null);

  protected get isEdit(): boolean {
    return this.id() !== undefined;
  }

  // Validation rules mirror the [Required]/[StringLength]/[Range] attributes in the C# DTOs.
  protected readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(150)]],
    description: ['', [Validators.maxLength(500)]],
    durationMinutes: [120, [Validators.required, Validators.min(1), Validators.pattern(/^\d+$/)]],
    releaseDate: ['', [Validators.required]],
    language: ['', [Validators.required, Validators.maxLength(50)]],
    genre: ['', [Validators.required, Validators.maxLength(50)]],
  });

  ngOnInit(): void {
    const id = this.id();
    if (id === undefined) return;

    this.loading.set(true);
    this.api.getById(Number(id)).subscribe({
      next: (movie) => {
        this.form.patchValue({
          title: movie.title,
          description: movie.description ?? '',
          durationMinutes: movie.durationMinutes,
          releaseDate: movie.releaseDate,
          language: movie.language,
          genre: movie.genre,
        });
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(describeError(err));
        this.loading.set(false);
      },
    });
  }

  protected invalid(name: FieldName): boolean {
    const c = this.form.controls[name];
    return c.invalid && (c.touched || c.dirty);
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.getRawValue();
    const payload: MovieRequest = {
      title: v.title.trim(),
      description: v.description.trim() || null,
      durationMinutes: Number(v.durationMinutes),
      releaseDate: v.releaseDate,
      language: v.language.trim(),
      genre: v.genre.trim(),
    };

    this.saving.set(true);
    this.error.set(null);

    const id = this.id();
    const request$ = id !== undefined ? this.api.update(Number(id), payload) : this.api.create(payload);

    request$.subscribe({
      next: (movie) => this.router.navigate(['/movies', movie.movieId]),
      error: (err) => {
        this.error.set(describeError(err));
        this.saving.set(false);
      },
    });
  }
}
