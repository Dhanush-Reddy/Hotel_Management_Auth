import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';
import { RoomsService, Room } from '../../services/rooms.service';
import { AuthService } from '../../../../core/services/auth.service';
import { Subject, debounceTime } from 'rxjs';

@Component({
  selector: 'app-rooms-list',
  templateUrl: './rooms-list.component.html',
  styleUrls: ['./rooms-list.component.scss']
})
export class RoomsListComponent implements OnInit {
  data = new MatTableDataSource<Room>([]);
  cols = ['id','roomNumber','capacity','status','actions'];
  q = ''; status = ''; loading = false;
  private filter$ = new Subject<void>();

  constructor(private api: RoomsService, private router: Router, public auth: AuthService) {}
  ngOnInit(){
    // Initial load
    this.load();
    // Debounced auto-search when filters change
    this.filter$.pipe(debounceTime(300)).subscribe(() => this.load());
  }

  onFilterChange(){
    this.filter$.next();
  }

  load(){
    this.loading = true;
    this.api.list(this.q, this.status).subscribe({
      next: d => { this.data.data = d.sort((a,b)=>a.id-b.id); this.loading = false; },
      error: _ => { this.loading = false; }
    });
  }

  newRoom(){ this.router.navigate(['/rooms/new']); }
  edit(r: Room){ this.router.navigate(['/rooms', r.id, 'edit']); }
  del(r: Room){
    if (!confirm(`Delete room ${r.roomNumber}?`)) return;
    this.api.remove(r.id).subscribe({ next: _ => this.load() });
  }
  setStatus(r: Room, status:'Available'|'Occupied'|'OutOfService'){
    this.api.setStatus(r.id, { status }).subscribe({ next: _ => this.load() });
  }
}
