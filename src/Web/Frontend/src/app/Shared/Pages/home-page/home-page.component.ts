import { Component, OnInit } from '@angular/core';
import { NavigationService } from '../../Services/navigation.service';
import { ConfigurationService } from '../../Services/configuration.service';
import { ColorConfiguration } from '../../Models/color-configuration';
import { DesktopService } from '../../Services/desktop.service';
import { ToastService } from '../../Services/toast.service';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.scss']
})
export class HomePageComponent implements OnInit {

  switchLeftLoading = false;
  switchRightLoading = false;

  constructor(
    private navigationService: NavigationService,
    private desktopService: DesktopService,
    private toastService: ToastService
  ) { }

  ngOnInit() {
    this.navigationService.enableNavigationBar();
  }

  switchLeft() {
    this.switchLeftLoading = true;
    this.desktopService.SwitchLeft(true).subscribe({
      next: result => {
        this.toastService.success("Der Desktop wurde gewechselt.");
        console.log(result);
      },
      complete: () => this.switchLeftLoading = false
    });
  }

  switchRight() {
    this.switchRightLoading = true;
    this.desktopService.SwitchRight(true).subscribe({
      next: result => {
        this.toastService.success("Der Desktop wurde gewechselt.");
        console.log(result);
      },
      complete: () => this.switchRightLoading = false
    });
  }

}
