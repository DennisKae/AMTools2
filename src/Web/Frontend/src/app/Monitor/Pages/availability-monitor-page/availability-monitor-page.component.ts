import { Component, OnInit } from '@angular/core';
import { NavigationService } from 'src/app/Shared/Services/navigation.service';
import { ColorConfiguration } from 'src/app/Shared/Models/color-configuration';
import { ConfigurationService } from 'src/app/Shared/Services/configuration.service';
import { ActivatedRoute } from '@angular/router';
import { SubscriberViewModel } from 'src/app/Shared/Models/subscriber-view-model';
import { SubscriberService } from 'src/app/Shared/Services/subscriber.service';
import { SettingsService } from 'src/app/Shared/Services/settings.service';
import { GuiVisibilityViewModel } from 'src/app/Shared/Models/gui-visibility-view-model';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { UtilService } from 'src/app/Shared/Services/util.service';
import { QualificationSettingViewModel } from 'src/app/Shared/Models/Settings/qualification-setting-view-model';
import { QualificationDisplayModel } from '../../Models/qualification-display-model';

@Component({
  selector: 'app-availability-monitor-page',
  templateUrl: './availability-monitor-page.component.html',
  styleUrls: ['./availability-monitor-page.component.scss']
})
export class AvailabilityMonitorPageComponent implements OnInit {

  visibility: GuiVisibilityViewModel;

  unfilteredSubscribers: SubscriberViewModel[];

  subscribers: SubscriberViewModel[];

  qualifications: QualificationSettingViewModel[];

  qualificationDisplayModels: QualificationDisplayModel[];

  lastUpdate: Date = null;

  loading = false;

  errorText: string;

  private hubConnection: HubConnection;

  constructor(
    private navigationService: NavigationService,
    private route: ActivatedRoute,
    private settingsService: SettingsService,
    private util: UtilService
  ) { }

  ngOnInit() {

    const ignoreFullscreen: boolean = this.route.snapshot.paramMap.get('ignoreFullscreen') === "true";
    if (!ignoreFullscreen) {
      this.navigationService.disableNavigationBar();
    }

    this.loading = true;
    this.settingsService.GetGuiVisibility().subscribe({
      next: visibility => {
        this.visibility = visibility;
        if (!this.util.isNullOrUndefined(visibility) && visibility.GroupSubscribersByQualification) {
          this.settingsService.GetAllQualificationSettings().subscribe({
            next: qualificationSettings => {
              this.qualifications = qualificationSettings;
              this.initializeHubConnection();
            },
            error: () => this.loading = false
          });
        } else {
          this.initializeHubConnection();
        }
      },
      error: () => this.loading = false
    });
  }

  initializeHubConnection() {
    this.hubConnection = new HubConnectionBuilder().withUrl(this.util.getAbsoluteUrlFromRelativeUrl("availability")).build();
    // Timeout 1440000ms = 24h
    this.hubConnection.serverTimeoutInMilliseconds = 1440000;
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
      this.errorText = null;
      this.unfilteredSubscribers = subscribers;
      this.subscribers = this.getFilteredSubscribers(subscribers);
      this.qualificationDisplayModels = this.getQualificationDisplayModels();

      this.lastUpdate = new Date();
    });

    this.hubConnection.onclose(() => {
      this.errorText = "Die Verbindung zum Server wurde beendet.";
      this.subscribers = null;
      this.unfilteredSubscribers = null;
      this.qualificationDisplayModels = null;
      this.lastUpdate = null;
    });
  }

  toggleNavigationBar() {
    if (this.navigationService.showLeftNavigationBar.value || this.navigationService.showTopNavigationBar.value) {
      this.navigationService.disableNavigationBar();
    } else {
      this.navigationService.enableNavigationBar();
    }
  }

  getFilteredSubscribers(subscribers: SubscriberViewModel[]): SubscriberViewModel[] {
    if (this.util.isNullOrUndefined(subscribers) || subscribers.length == 0) {
      return null;
    }

    if (this.util.isNullOrUndefined(this.visibility)) {
      return subscribers;
    }

    if (this.visibility.SortSubscribersByName) {
      subscribers = subscribers.sort((a, b) => {
        if (a.Name < b.Name) {
          return -1;
        }
        if (a.Name > b.Name) {
          return 1;
        }
        return 0;
      });
    }

    return subscribers;
  }

  getQualificationDisplayModels(): QualificationDisplayModel[] {
    if (this.util.isNullOrUndefined(this.qualifications) || this.qualifications.length === 0 || this.util.isNullOrUndefined(this.subscribers) || this.subscribers.length === 0) {
      return null;
    }
    let allSubscribers = this.subscribers.slice(0);

    let result: QualificationDisplayModel[] = [];
    this.qualifications.forEach(qualification => {
      let newResultItem = new QualificationDisplayModel();
      newResultItem.Qualification = qualification;
      newResultItem.Subscribers = allSubscribers.filter(x => !this.util.isNullOrUndefined(x.Qualifications) && x.Qualifications.some(y => y.Abkuerzung === qualification.Abkuerzung));

      result.push(newResultItem);

      if (!this.util.isNullOrUndefined(newResultItem.Subscribers) && newResultItem.Subscribers.length > 0) {
        // Verwendete Subscriber herausfiltern
        allSubscribers = allSubscribers.filter(x => !newResultItem.Subscribers.some(y => y.Issi === x.Issi));
      }
    });

    return result;
  }

}
