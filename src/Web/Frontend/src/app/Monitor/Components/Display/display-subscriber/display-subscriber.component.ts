import { Component, OnInit, Input } from '@angular/core';
import { SubscriberViewModel } from 'src/app/Shared/Models/subscriber-view-model';

@Component({
  selector: 'app-display-subscriber',
  templateUrl: './display-subscriber.component.html',
  styleUrls: ['./display-subscriber.component.scss']
})
export class DisplaySubscriberComponent implements OnInit {

  @Input() subscriber: SubscriberViewModel;

  @Input() showAvailabilityTimestamp = true;

  constructor() { }

  ngOnInit() {
  }

}
