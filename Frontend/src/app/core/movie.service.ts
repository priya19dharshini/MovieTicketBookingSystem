import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Movie, MovieRequest } from './movie.model';

@Injectable({ providedIn: 'root' })
export class MovieService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/movies`;

  /** GET api/movies */
  getAll(): Observable<Movie[]> {
    return this.http.get<Movie[]>(this.baseUrl);
  }

  /** GET api/movies/{id} */
  getById(id: number): Observable<Movie> {
    return this.http.get<Movie>(`${this.baseUrl}/${id}`);
  }

  /** GET api/movies/search?title=... */
  search(title: string): Observable<Movie[]> {
    const params = new HttpParams().set('title', title);
    return this.http.get<Movie[]>(`${this.baseUrl}/search`, { params });
  }

  /** POST api/movies */
  create(request: MovieRequest): Observable<Movie> {
    return this.http.post<Movie>(this.baseUrl, request);
  }

  /** PUT api/movies/{id} */
  update(id: number, request: MovieRequest): Observable<Movie> {
    return this.http.put<Movie>(`${this.baseUrl}/${id}`, request);
  }

  /** DELETE api/movies/{id} */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
