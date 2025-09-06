import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GuestsService, CreateGuest } from '../../services/guests.service';

@Component({
  selector: 'app-guest-form',
  templateUrl: './guest-form.component.html',
  styleUrls: ['./guest-form.component.scss']
})
export class GuestFormComponent implements OnInit {
  id?: number;
  model: CreateGuest = { fullName:'', phone:'', email:'', idProof:'' };
  saving = false;

  constructor(private route: ActivatedRoute, private router: Router, private api: GuestsService) {}

  ngOnInit(){
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.id = +id;
      this.api.get(this.id).subscribe({
        next: g => this.model = {
          fullName: g.fullName,
          phone: g.phone ?? '',
          email: g.email ?? '',
          idProof: g.idProof ?? ''
        }
      });
    }
  }

  save(){
    this.saving = true;
    const done = () => { this.saving = false; this.router.navigate(['/guests']); };
    if (this.id) this.api.update(this.id, this.model).subscribe({ next: done, error: _ => this.saving=false });
    else this.api.create(this.model).subscribe({ next: done, error: _ => this.saving=false });
  }
}
