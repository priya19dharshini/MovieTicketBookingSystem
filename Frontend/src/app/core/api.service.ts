import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(private http: HttpClient) {}
  movies(title = '') { let p = new HttpParams().set('pageNumber', 1).set('pageSize', 100); if (title) p = p.set('title', title); return this.http.get<any[]>('/api/movies', { params: p }); }
  movie(id: number) { return this.http.get<any>(`/api/movies/${id}`); }
  createMovie(x: any) { return this.http.post<any>('/api/movies', x); }
  updateMovie(id: number, x: any) { return this.http.put<any>(`/api/movies/${id}`, x); }
  deleteMovie(id: number) { return this.http.delete(`/api/movies/${id}`); }
  theatres() { return this.http.get<any[]>('/api/theatres?pageNumber=1&pageSize=100'); }
  createTheatre(x: any) { return this.http.post<any>('/api/theatres', x); }
  updateTheatre(id: number, x: any) { return this.http.put<any>(`/api/theatres/${id}`, x); }
  deleteTheatre(id: number) { return this.http.delete(`/api/theatres/${id}`); }
  screens(theatreId?: number) { return this.http.get<any[]>('/api/screens', { params: theatreId ? { theatreId } : {} }); }
  createScreen(x: any) { return this.http.post<any>('/api/screens', x); }
  updateScreen(id: number, x: any) { return this.http.put<any>(`/api/screens/${id}`, x); }
  deleteScreen(id: number) { return this.http.delete(`/api/screens/${id}`); }
  seats(screenId: number) { return this.http.get<any[]>(`/api/seats/screen/${screenId}`); }
  bulkSeats(x: any) { return this.http.post<any[]>('/api/seats/bulk', x); }
  shows(filters: any = {}) { let p = new HttpParams(); Object.keys(filters).forEach(k => { if (filters[k] !== null && filters[k] !== undefined && filters[k] !== '') p = p.set(k, filters[k]); }); return this.http.get<any[]>('/api/shows', { params: p }); }
  showsForMovie(id: number) { return this.http.get<any[]>(`/api/shows/movie/${id}`); }
  showSeats(id: number) { return this.http.get<any[]>(`/api/shows/${id}/seats`); }
  createShow(x: any) { return this.http.post<any>('/api/shows', x); }
  updateShow(id: number, x: any) { return this.http.put<any>(`/api/shows/${id}`, x); }
  deleteShow(id: number) { return this.http.delete(`/api/shows/${id}`); }
  createBooking(x: any) { return this.http.post<any>('/api/bookings', x); }
  booking(id: number) { return this.http.get<any>(`/api/bookings/${id}`); }
  cancelBooking(id: number) { return this.http.put<any>(`/api/bookings/${id}/cancel`, {}); }
  allBookings() { return this.http.get<any[]>('/api/bookings'); }
}
