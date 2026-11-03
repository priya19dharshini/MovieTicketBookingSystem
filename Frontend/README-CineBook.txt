CINEBOOK ANGULAR FRONTEND
========================
Built from the Angular project supplied by the user and matched to the supplied ASP.NET Core API controllers/DTOs.

REQUIREMENTS
- Node.js 20.19+ or 22.12+
- npm
- ASP.NET Core backend running at https://localhost:7272 (or update proxy.conf.json)

RUN
1. Extract this ZIP to a simple path without special characters, for example D:\Projects\CineBook-Angular-Frontend.
2. Open the folder containing package.json in VS Code.
3. In PowerShell, if Node is installed at D:\downloads:
   $env:Path = "D:\downloads;" + $env:Path
4. Install dependencies:
   npm install
5. Start Angular:
   npx ng serve --proxy-config proxy.conf.json
6. Browse http://localhost:4200

BACKEND
The proxy.conf.json forwards /api to https://localhost:7272. If your backend uses a different HTTPS port, update the target. Run the backend separately.

FEATURES
- Customer registration and login
- JWT bearer token interceptor and role-aware navigation
- Movie listing, search, details, admin create/update/delete
- Public theatre and show discovery
- Admin theatre creation
- Admin/TheatreManager screen creation and bulk seat generation
- Admin/TheatreManager show scheduling
- Customer seat selection and booking submission
- Booking confirmation and recent booking lookup by saved booking ID
- Admin booking list
- Responsive dark CineBook theme

IMPORTANT BACKEND LIMITATIONS (not fixable in frontend alone)
1. BookingsController has no GET endpoint for a customer's booking history. This frontend can retrieve the most recent booking ID saved in the same browser, but cannot display full booking history.
2. BookingsController comments say ownership verification will be added, but GetById and Cancel currently do not enforce ownership in the supplied backend. Add server-side ownership checks before production use.
3. TheatreManager authorization currently allows the role to operate on screens/seats/shows, but the supplied backend does not scope operations to a manager's assigned theatre. Add server-side theatre assignment checks before production use.
4. There is no payment API in the supplied backend. Booking confirmation does not process payment.
5. Public registration creates the backend's default Customer role. Admin and TheatreManager accounts must be provisioned by the backend seeder/admin workflow; never expose role selection in public registration.
6. The backend RoleSeeder requires SeedUsers:AdminPassword and SeedUsers:ManagerPassword in development configuration/User Secrets. Configure those before backend startup.

SECURITY
The browser-side route/menu checks improve the user experience only. ASP.NET Core [Authorize] policies are the security boundary. Never rely on Angular role checks to protect data.
