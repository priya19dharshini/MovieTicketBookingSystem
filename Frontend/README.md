# CineBook – Movie Ticket Booking (Angular frontend)

Angular 21 (standalone components, signals, reactive forms) frontend for the
**MovieTicketBookingSystem** ASP.NET Core Web API.

Features: list movies · search by title · filter by genre · view details · add · edit · delete (with confirmation).

## Prerequisites
- Node.js 22.22.3+ or 24.15+ (check with `node -v`)
- Angular CLI: `npm install -g @angular/cli`
- The .NET backend running (Visual Studio ▶ or `dotnet run`)

## Run locally
```bash
npm install
npm start          # = ng serve, opens http://localhost:4200
```

During development every request to `/api/...` is **proxied** to the backend by
`proxy.conf.json`, so you don't need to configure CORS to try it out.

### Which backend port?
`proxy.conf.json` points to the HTTPS profile (`https://localhost:7272`).
If you run the **http** profile instead, change the target to `http://localhost:5263`
and restart `npm start`.

## Project structure
```
src/app/
  core/            movie.model.ts · movie.service.ts · http-error.ts · duration.pipe.ts
  pages/
    movie-list/    search, genre filter, cards, delete dialog
    movie-form/    add + edit (same form, validation mirrors the C# DTOs)
    movie-detail/  single movie view
src/environments/  environment.ts (dev) · environment.prod.ts (production)
```

## API endpoints used
| Action | Request |
|---|---|
| List | `GET /api/movies` |
| Search | `GET /api/movies/search?title=...` |
| Details | `GET /api/movies/{id}` |
| Create | `POST /api/movies` |
| Update | `PUT /api/movies/{id}` |
| Delete | `DELETE /api/movies/{id}` |

## Production build
1. Set `apiUrl` in `src/environments/environment.prod.ts` to your deployed API (e.g. `https://api.example.com/api`).
2. Enable CORS in the backend for the frontend's origin (see below).
3. `npm run build` → output in `dist/movie-ticket-booking-ui/browser`.

### CORS in the ASP.NET Core backend (needed for production, or if you skip the proxy)
In `Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy => policy
        .WithOrigins("http://localhost:4200")   // add your deployed frontend URL too
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();
// ...
app.UseHttpsRedirection();
app.UseCors("Frontend");      // must come before MapControllers
app.MapControllers();
```
