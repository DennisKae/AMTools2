import { Component, OnInit, AfterViewInit } from '@angular/core';
import { NavigationService } from './Shared/Services/navigation.service';
import { ColorConfiguration } from './Shared/Models/color-configuration';
import { ConfigurationService } from './Shared/Services/configuration.service';
import { SwUpdate } from '@angular/service-worker';
import { NotificationService } from './Shared/Services/notification.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  showLeftNavigationBar: boolean;
  showTopNavigationBar: boolean;

  colorConfiguration: ColorConfiguration;

  constructor(
    private navigationService: NavigationService,
    private configurationService: ConfigurationService,
    private notificationService: NotificationService,
    private swUpdate: SwUpdate
  ) {
  }

  ngOnInit(): void {
    this.navigationService.showLeftNavigationBar.subscribe(
      result => this.showLeftNavigationBar = result
    );

    this.navigationService.showTopNavigationBar.subscribe(
      result => this.showTopNavigationBar = result
    );

    this.configurationService.colorConfiguration.subscribe(
      result => this.colorConfiguration = result
    );
    console.log("ngOnInit: this.swUpdate.isEnabled?", this.swUpdate.isEnabled);

    if (!this.swUpdate.isEnabled) {
      this.notificationService.info("Hinweis: Der eingesetzte Browser unterstützt keine Online-Funktionen!");
    } else {
      this.detectUpdates();
    }
  }

  private detectUpdates(): void {
    this.swUpdate.available.subscribe(() => {
      // Update wurde entdeckt
      // Update herunterladen
      this.swUpdate.activateUpdate().then(() => {
        // Update wurde heruntergeladen
        this.showUpdateRequiredDialog();
      });
    });

    // Auf Updates prüfen
    this.swUpdate.checkForUpdate();
  }

  private showUpdateRequiredDialog() {
    const swalWithBootstrapButtons = Swal.mixin({
      customClass: {
        confirmButton: 'btn btn-success mx-2',
        cancelButton: 'btn btn-danger mx-2'
      },
      buttonsStyling: false
    })

    swalWithBootstrapButtons.fire({
      title: 'Es steht ein Update zur Verfügung.',
      icon: 'warning',
      allowOutsideClick: false,
      confirmButtonText: 'Update laden',
    }).then(() => {
      location.reload();
    });
  }
}
