import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GuestsService, Guest } from '../../services/guests.service';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-guests-list',
  templateUrl: './guests-list.component.html',
  styleUrls: ['./guests-list.component.scss']
})
export class GuestsListComponent implements OnInit {
  data = new MatTableDataSource<Guest>([]);
  cols = ['id','fullName','phone','email','actions'];
  q = ''; page = 1; pageSize = 20; loading = false;

  constructor(private api: GuestsService, private router: Router) {}
  ngOnInit(){ this.load(); }

  load(){
    this.loading = true;
    this.api.list(this.page, this.pageSize, this.q).subscribe({
      next: d => { this.data.data = d.sort((a,b)=>a.id-b.id); this.loading = false; },
      error: _ => this.loading = false
    });
  }
  newGuest(){ this.router.navigate(['/guests/new']); }
  edit(g: Guest){ this.router.navigate(['/guests', g.id, 'edit']); }
  del(g: Guest){
    if (!confirm(`Delete guest ${g.fullName}?`)) return;
    this.api.remove(g.id).subscribe({ next: _ => this.load() });
  }
}
