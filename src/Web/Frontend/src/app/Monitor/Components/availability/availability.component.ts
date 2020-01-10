import { Component, OnInit, Input, OnDestroy, Output, EventEmitter } from '@angular/core';
import { UtilService } from 'src/app/Shared/Services/util.service';
import { Constants } from 'src/app/Shared/Models/constants';
import { ColorConfiguration } from 'src/app/Shared/Models/color-configuration';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { SubscriberViewModel } from 'src/app/Shared/Models/subscriber-view-model';

@Component({
  selector: 'app-availability',
  templateUrl: './availability.component.html',
  styleUrls: ['./availability.component.scss']
})
export class AvailabilityComponent implements OnInit, OnDestroy {

  subscribers: SubscriberViewModel[];

  loading = false;

  errorText: string = null;


  @Input() colorConfiguration: ColorConfiguration = Constants.DEFAULT_COLOR_CONFIGURATION;

  @Output() lastUpdate: EventEmitter<Date> = new EventEmitter();

  private hubConnection: HubConnection

  constructor(
    private util: UtilService
  ) { }

  ngOnInit() {
    this.hubConnection = new HubConnectionBuilder().withUrl(this.util.getAbsoluteUrlFromRelativeUrl("availability")).build();

    this.hubConnection
      .start()
      .then(() => {
        console.log("Hub Connection started!");
        this.loading = true;
      })
      .catch(error => {
        console.log("Error while establishing the hub connection.", error);
        this.errorText = "Bei der Herstellung der Verbindung zum Server ist ein Fehler aufgetreten.";
      });

    this.hubConnection.on('SendToAll', (subscribers: SubscriberViewModel[]) => {
      this.loading = false;
      this.subscribers = subscribers;

      this.lastUpdate.emit(new Date(Date.now()));
    });

    this.hubConnection.onclose(() => {
      this.errorText = "Die Verbindung zum Server wurde aufgrund eines Fehlers beendet.";
      this.subscribers = null;
    });
  }

  ngOnDestroy() {
    if (!this.util.isNullOrUndefined(this.hubConnection)) {
      this.hubConnection.stop();
    }
  }

}
