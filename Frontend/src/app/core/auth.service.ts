
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';

export type AppRole = 'Customer' | 'Admin' | 'TheatreManager' | '';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly key = 'cinebook_token';

  constructor(private http: HttpClient) {}

  register(data: {
    fullName: string;
    email: string;
    password: string;
  }) {
    return this.http.post<{ message: string }>(
      '/api/auth/register',
      data
    );
  }

  login(data: { email: string; password: string }) {
    return this.http
      .post<{ message: string; token: string }>(
        '/api/auth/login',
        data
      )
      .pipe(
        tap(response =>
          localStorage.setItem(this.key, response.token)
        )
      );
  }

  get token(): string | null {
    return localStorage.getItem(this.key);
  }

  logout(): void {
    localStorage.removeItem(this.key);
  }

  get claims(): Record<string, any> {
    try {
      const payload = this.token?.split('.')[1];

      return payload
        ? JSON.parse(
            atob(
              payload
                .replace(/-/g, '+')
                .replace(/_/g, '/')
            )
          )
        : {};
    } catch {
      return {};
    }
  }

  get role(): AppRole {
    const c = this.claims;

    return (
      c['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
      c['role'] ||
      c['Role'] ||
      ''
    ) as AppRole;
  }

  get name(): string {
    const c = this.claims;

    return (
      c['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
      c['unique_name'] ||
      c['name'] ||
      'CineBook user'
    );
  }

  get email(): string {
    const c = this.claims;

    return (
      c['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ||
      c['email'] ||
      ''
    );
  }

  get loggedIn(): boolean {
    return !!this.token;
  }
}
