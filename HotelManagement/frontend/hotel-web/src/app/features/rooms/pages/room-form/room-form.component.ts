import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RoomsService, CreateRoom, UpdateRoom } from '../../services/rooms.service';

@Component({
  selector: 'app-room-form',
  templateUrl: './room-form.component.html',
  styleUrls: ['./room-form.component.scss']
})
export class RoomFormComponent implements OnInit {
  id?: number;
  model: { roomNumber: string; capacity: number } = { roomNumber: '', capacity: 2 };
  saving = false;

  constructor(private route: ActivatedRoute, private router: Router, private api: RoomsService) {}

  ngOnInit(){
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.id = +id;
      this.api.get(this.id).subscribe({ next: r => this.model = { roomNumber: r.roomNumber, capacity: r.capacity } });
    }
  }

  save(){
    this.saving = true;
    const done = () => { this.saving = false; this.router.navigate(['/rooms']); };
    if (this.id) {
      const body: UpdateRoom = { roomNumber: this.model.roomNumber, capacity: this.model.capacity };
      this.api.update(this.id, body).subscribe({ next: done, error: _ => this.saving = false });
    } else {
      const body: CreateRoom = { roomNumber: this.model.roomNumber, capacity: this.model.capacity };
      this.api.create(body).subscribe({ next: done, error: _ => this.saving = false });
    }
  }
}

