import { HttpErrorResponse } from '@angular/common/http';

/** Turns whatever the ASP.NET Core API returned into a readable message. */
export function describeError(err: unknown): string {
  if (!(err instanceof HttpErrorResponse)) {
    return 'Something went wrong. Please try again.';
  }

  if (err.status === 0) {
    return 'Cannot reach the server. Make sure the backend API is running.';
  }
  if (err.status === 404) {
    return 'The requested movie was not found.';
  }

  const body = err.error;

  // [ApiController] validation errors: { errors: { Field: ["msg"] } }
  if (body && typeof body === 'object' && body.errors) {
    const messages = Object.values(body.errors as Record<string, string[]>).flat();
    if (messages.length) return messages.join(' ');
  }
  // BadRequest("text") returns a plain string
  if (typeof body === 'string' && body.trim()) {
    return body;
  }
  if (body && typeof body === 'object' && body.title) {
    return String(body.title);
  }

  return `Request failed (${err.status}).`;
}
