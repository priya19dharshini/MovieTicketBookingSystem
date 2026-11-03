/** Mirrors MovieResponse.cs */
export interface Movie {
  movieId: number;
  title: string;
  description: string | null;
  durationMinutes: number;
  /** DateOnly from .NET is serialised as "yyyy-MM-dd" */
  releaseDate: string;
  language: string;
  genre: string;
}

/** Mirrors CreateMovieRequest.cs / UpdateMovieRequest.cs (identical shapes) */
export interface MovieRequest {
  title: string;
  description: string | null;
  durationMinutes: number;
  releaseDate: string;
  language: string;
  genre: string;
}
