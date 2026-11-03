import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, NavigationEnd, Router, RouterLink } from '@angular/router';
import { filter } from 'rxjs';
import { AuthService } from '../../core/auth.service';
import { ApiService } from '../../core/api.service';

@Component({ selector: 'app-portal', standalone: true, imports: [CommonModule, FormsModule, RouterLink], templateUrl: './portal.html' })
export class PortalComponent implements OnInit {
 private readonly changeDetector = inject(ChangeDetectorRef);
 showError = '';
 page = ''; loading = false; busy = false; error = ''; notice = ''; query = ''; movies: any[] = []; theatres: any[] = []; screens: any[] = []; shows: any[] = []; seats: any[] = []; bookings: any[] = [];
 selectedMovie: any = null; selectedShow: any = null; selectedSeats: number[] = []; lastBooking: any = null; bookingId = 0; editId = 0;
 authForm = { fullName: '', email: '', password: '' }; movieForm: any = { title:'', description:'', durationMinutes:120, releaseDate:'', language:'Tamil', genre:'Drama' };
 theatreForm = { name:'', location:'' }; screenForm: any = { theatreId:0, screenName:'' }; showForm: any = { movieId:0, screenId:0, startTime:'', endTime:'', ticketPrice:150 }; seatForm: any = { screenId:0, rowName:'A', startNumber:1, count:10, seatType:'Regular' };
 bookingForm = { customerName:'', customerEmail:'', customerPhone:'' };
 constructor(public auth: AuthService, private api: ApiService, private router: Router, private route: ActivatedRoute) {}
 ngOnInit(){ this.syncPage(); this.router.events.pipe(filter(e => e instanceof NavigationEnd)).subscribe(() => { this.syncPage(); }); }
 private syncPage(){ const p=this.router.url.split('?')[0]; this.page=p; this.error=''; this.notice=''; const adminOnly=['/movies/new','/theatres','/bookings']; const managerAllowed=['/screens','/shows']; if((adminOnly.includes(p)||p.endsWith('/edit')) && this.auth.role!=='Admin'){ this.router.navigateByUrl(this.auth.loggedIn?'/access-denied':'/login'); return; } if(managerAllowed.includes(p) && !['Admin','TheatreManager'].includes(this.auth.role)){ this.router.navigateByUrl(this.auth.loggedIn?'/access-denied':'/login'); return; } if(p.startsWith('/book/') && !['Customer','Admin'].includes(this.auth.role)){ this.router.navigateByUrl(this.auth.loggedIn?'/access-denied':'/login'); return; } if(p==='/bookings' && this.auth.role!=='Admin'){this.router.navigateByUrl('/access-denied');return;} if(p.startsWith('/movies/')) { const id=Number(p.split('/')[2]); if(id && p.split('/').length===3) this.loadMovie(id); else if(p==='/movies/new') this.loadMovies(); else if(p.includes('/edit')) { this.api.movie(id).subscribe({next:m=>{this.selectedMovie=m;this.editId=id;this.movieForm={title:m.title,description:m.description||'',durationMinutes:m.durationMinutes,releaseDate:m.releaseDate,language:m.language,genre:m.genre};},error:e=>this.fail(e)}); } else this.loadMovies(); } else if(p==='/movies'||p==='/') this.loadMovies(); else if(p==='/admin'||p==='/manager') this.loadDashboard(); else if(p==='/theatres') this.loadTheatres(); else if(p==='/screens') {this.loadTheatres();this.loadScreens();} else if(p==='/shows') {this.loadMovies();this.loadTheatres();this.loadScreens();this.loadShows();} else if(p.startsWith('/book/')) { const id=Number(p.split('/')[2]); this.api.shows().subscribe({next:ss=>{this.selectedShow=ss.find(x=>x.showId===id)||null;},error:e=>this.fail(e)}); this.loading=true; this.api.showSeats(id).subscribe({next:r=>{this.seats=r;this.loading=false;},error:e=>this.fail(e)}); } else if(p==='/booking-confirmation') this.loadLastBooking(); else if(p==='/my-bookings') this.loadLastBooking(); else if(p==='/bookings') this.loadBookings(); else if(p==='/login'||p==='/register'){} }
 private fail(e:any){ this.error=e?.error?.message || (typeof e?.error==='string'?e.error:'Request failed. Check that the backend is running and try again.'); this.loading=false; this.busy=false; }
 
private loadMovies() {
  this.loading = true;
  this.error = '';

  this.api.movies(this.query).subscribe({
    next: (r) => {
      this.movies = Array.isArray(r)
        ? r
        : (r as any)?.items || [];

      this.loading = false;
      this.changeDetector.markForCheck();
    },
    error: (e) => {
      this.fail(e);
      this.changeDetector.markForCheck();
    }
  });
}

 search(){this.loadMovies();}
 loadMovie(id:number){this.loading=true;this.selectedMovie=null;this.shows=[];this.showError='';this.api.movie(id).subscribe({next:m=>{this.selectedMovie=m;this.loading=false;this.changeDetector.markForCheck();this.api.showsForMovie(id).subscribe({next:s=>{this.shows=Array.isArray(s)?s:[];this.showError='';this.changeDetector.markForCheck();},error:()=>{this.showError='Showtimes are temporarily unavailable. Please try again later.';this.changeDetector.markForCheck();}});},error:()=>{this.loading=false;this.error='Unable to load this movie. Please try again.';this.changeDetector.markForCheck();}});}
 loadTheatres(){this.api.theatres().subscribe({next:r=>this.theatres=Array.isArray(r)?r:[],error:e=>this.fail(e)});}
 loadScreens(){this.api.screens().subscribe({next:r=>this.screens=Array.isArray(r)?r:[],error:e=>this.fail(e)});}
 loadShows(){this.api.shows().subscribe({next:r=>this.shows=Array.isArray(r)?r:[],error:e=>this.fail(e)});}
 loadDashboard(){this.loadMovies();this.loadTheatres();this.loadShows();}
 loadBookings(){this.loading=true;this.api.allBookings().subscribe({next:r=>{this.bookings=Array.isArray(r)?r:[];this.loading=false;},error:e=>this.fail(e)});}
 loadLastBooking(){const id=Number(localStorage.getItem('cinebook_last_booking'));if(id){this.api.booking(id).subscribe({next:b=>this.lastBooking=b,error:e=>this.fail(e)});} }
 submitAuth(){this.error='';this.busy=true;if(this.page==='/register'){this.auth.register(this.authForm).subscribe({next:r=>{this.notice=r.message||'Registration successful. Please sign in.';this.busy=false;this.router.navigateByUrl('/login');},error:e=>this.fail(e)});}else{this.auth.login({email:this.authForm.email,password:this.authForm.password}).subscribe({next:()=>{this.busy=false;const role=this.auth.role;this.router.navigateByUrl(role==='Admin'?'/admin':role==='TheatreManager'?'/manager':'/movies');},error:e=>this.fail(e)});}}
 can(roles:string[]){return roles.includes(this.auth.role);}
 saveMovie(){this.busy=true;this.error='';const req={...this.movieForm,durationMinutes:Number(this.movieForm.durationMinutes),description:this.movieForm.description||null};const op=this.editId?this.api.updateMovie(this.editId,req):this.api.createMovie(req);op.subscribe({next:()=>{this.busy=false;this.notice='Movie saved successfully.';this.router.navigateByUrl('/movies');},error:e=>this.fail(e)});}
 editMovie(m:any){this.editId=m.movieId;this.movieForm={title:m.title,description:m.description||'',durationMinutes:m.durationMinutes,releaseDate:m.releaseDate,language:m.language,genre:m.genre};this.router.navigateByUrl(`/movies/${m.movieId}/edit`);}
 deleteMovie(id:number){if(!confirm('Delete this movie?'))return;this.api.deleteMovie(id).subscribe({next:()=>this.loadMovies(),error:e=>this.fail(e)});}
 deleteTheatre(id:number){if(!confirm('Delete this theatre?'))return;this.api.deleteTheatre(id).subscribe({next:()=>this.loadTheatres(),error:e=>this.fail(e)});}
 deleteScreen(id:number){if(!confirm('Delete this screen?'))return;this.api.deleteScreen(id).subscribe({next:()=>this.loadScreens(),error:e=>this.fail(e)});}
 deleteShow(id:number){if(!confirm('Delete this show?'))return;this.api.deleteShow(id).subscribe({next:()=>this.loadShows(),error:e=>this.fail(e)});}
 saveTheatre(){this.busy=true;this.api.createTheatre(this.theatreForm).subscribe({next:()=>{this.busy=false;this.theatreForm={name:'',location:''};this.loadTheatres();this.notice='Theatre created.';},error:e=>this.fail(e)});}
 saveScreen(){this.busy=true;this.api.createScreen({theatreId:Number(this.screenForm.theatreId),screenName:this.screenForm.screenName}).subscribe({next:()=>{this.busy=false;this.loadScreens();this.notice='Screen created.';},error:e=>this.fail(e)});}
 generateSeats(){const n=Number(this.seatForm.count);const seats=Array.from({length:n},(_,i)=>({rowName:this.seatForm.rowName,seatNumber:Number(this.seatForm.startNumber)+i,seatType:this.seatForm.seatType}));this.busy=true;this.api.bulkSeats({screenId:Number(this.seatForm.screenId),seats}).subscribe({next:()=>{this.busy=false;this.notice=`${n} seats created.`;},error:e=>this.fail(e)});}
 saveShow(){this.busy=true;const req={movieId:Number(this.showForm.movieId),screenId:Number(this.showForm.screenId),startTime:this.showForm.startTime,endTime:this.showForm.endTime,ticketPrice:Number(this.showForm.ticketPrice)};this.api.createShow(req).subscribe({next:()=>{this.busy=false;this.loadShows();this.notice='Show scheduled.';},error:e=>this.fail(e)});}
 chooseShow(s:any){this.selectedShow=s;this.selectedSeats=[];this.loading=true;this.api.showSeats(s.showId).subscribe({next:r=>{this.seats=r;this.loading=false;this.router.navigateByUrl(`/book/${s.showId}`);},error:e=>this.fail(e)});}
 toggleSeat(s:any){if(s.status?.toLowerCase()!=='available'&&s.status?.toLowerCase()!=='vacant')return;const id=Number(s.seatId);this.selectedSeats=this.selectedSeats.includes(id)?this.selectedSeats.filter(x=>x!==id):[...this.selectedSeats,id];}
 book(){if(!this.selectedShow||!this.selectedSeats.length){this.error='Select at least one available seat.';return;}this.busy=true;const req={...this.bookingForm,showId:this.selectedShow.showId,seatIds:this.selectedSeats};this.api.createBooking(req).subscribe({next:b=>{this.busy=false;this.lastBooking=b;localStorage.setItem('cinebook_last_booking',String(b.bookingId));this.router.navigateByUrl('/booking-confirmation');},error:e=>this.fail(e)});}
 cancelBooking(id:number){if(!confirm('Cancel this booking?'))return;this.api.cancelBooking(id).subscribe({next:b=>{this.lastBooking=b;this.notice='Booking cancelled.';},error:e=>this.fail(e)});}
 signOut(){this.auth.logout();this.router.navigateByUrl('/movies');}
}
