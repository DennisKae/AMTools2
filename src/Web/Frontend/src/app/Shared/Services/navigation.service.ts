import { Injectable } from '@angular/core';
import { Observable, Subject, BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NavigationService {

  // https://blog.angular-university.io/how-to-build-angular2-apps-using-rxjs-observable-data-services-pitfalls-to-avoid/

  showLeftNavigationBar: BehaviorSubject<boolean> = new BehaviorSubject(true);
  showTopNavigationBar: BehaviorSubject<boolean> = new BehaviorSubject(true);

  constructor() {
  }

  enableNavigationBar(): void {
    this.showLeftNavigationBar.next(true);
    this.showTopNavigationBar.next(true);
  }
  disableNavigationBar(): void {
    this.showLeftNavigationBar.next(false);
    this.showTopNavigationBar.next(false);
  }

  enableLeftNavigationBar(): void {
    this.showLeftNavigationBar.next(true);
  }
  disableLeftNavigationBar(): void {
    this.showLeftNavigationBar.next(false);
  }

  enableTopNavigationBar(): void {
    this.showTopNavigationBar.next(true);
  }
  disableTopNavigationBar(): void {
    this.showTopNavigationBar.next(false);
  }
}
