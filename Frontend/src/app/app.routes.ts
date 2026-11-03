import { Routes } from '@angular/router';
export const routes: Routes = [
 { path: '', pathMatch: 'full', redirectTo: 'movies' },
 { path: 'login', loadComponent: () => import('./pages/portal/portal').then(m => m.PortalComponent), data: { page: 'login' } },
 { path: 'register', loadComponent: () => import('./pages/portal/portal').then(m => m.PortalComponent), data: { page: 'register' } },
 { path: '**', loadComponent: () => import('./pages/portal/portal').then(m => m.PortalComponent) }
];
