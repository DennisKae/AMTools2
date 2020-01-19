import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { AvailabilityMonitorPageComponent } from './Monitor/Pages/availability-monitor-page/availability-monitor-page.component';
import { NotFoundPageComponent } from './Shared/Pages/not-found-page/not-found-page.component';
import { HomePageComponent } from './Shared/Pages/home-page/home-page.component';
import { AvailabilityComponent } from './Monitor/Components/availability/availability.component';
import { DisplaySubscriberComponent } from './Monitor/Components/Display/display-subscriber/display-subscriber.component';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';

@NgModule({
  declarations: [
    AppComponent,
    AvailabilityMonitorPageComponent,
    NotFoundPageComponent,
    HomePageComponent,
    AvailabilityComponent,
    DisplaySubscriberComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    CommonModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot(),
    ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment.production })
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
