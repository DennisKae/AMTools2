import { Component, OnInit } from '@angular/core';
import { NavigationService } from 'src/app/Shared/Services/navigation.service';
import { ColorConfiguration } from 'src/app/Shared/Models/color-configuration';
import { ConfigurationService } from 'src/app/Shared/Services/configuration.service';
import { ActivatedRoute } from '@angular/router';
import { SubscriberViewModel } from 'src/app/Shared/Models/subscriber-view-model';
import { SubscriberService } from 'src/app/Shared/Services/subscriber.service';
import { SettingsService } from 'src/app/Shared/Services/settings.service';
import { GuiVisibilityViewModel } from 'src/app/Shared/Models/gui-visibility-view-model';

@Component({
  selector: 'app-availability-monitor-page',
  templateUrl: './availability-monitor-page.component.html',
  styleUrls: ['./availability-monitor-page.component.scss']
})
export class AvailabilityMonitorPageComponent implements OnInit {

  visibility: GuiVisibilityViewModel;

  subscribers: SubscriberViewModel[];

  lastUpdate: Date = null;

  loading = false;

  constructor(
    private navigationService: NavigationService,
    private route: ActivatedRoute,
    private subscriberService: SubscriberService,
    private settingsService: SettingsService
  ) { }

  ngOnInit() {

    const ignoreFullscreen: boolean = this.route.snapshot.paramMap.get('ignoreFullscreen') === "true";
    if (!ignoreFullscreen) {
      this.navigationService.disableNavigationBar();
    }

    this.manuallyLoadSubscribers();
    // TODO: Das hier sinnvoll nutzen:
    this.loadGuiVisibility();
  }

  manuallyLoadSubscribers() {
    this.loading = true;
    this.subscriberService.GetAll().subscribe({
      next: result => this.subscribers = result,
      complete: () => {
        this.loading = false;
        this.lastUpdate = new Date();
      }
    });
  }

  loadGuiVisibility() {
    this.settingsService.GetGuiVisibility().subscribe({
      next: result => this.visibility = result
    });
  }

  toggleNavigationBar() {
    if (this.navigationService.showLeftNavigationBar.value || this.navigationService.showTopNavigationBar.value) {
      this.navigationService.disableNavigationBar();
    } else {
      this.navigationService.enableNavigationBar();
    }
  }

}
