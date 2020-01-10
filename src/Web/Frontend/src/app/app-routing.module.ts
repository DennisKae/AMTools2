import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AvailabilityMonitorPageComponent } from './Monitor/Pages/availability-monitor-page/availability-monitor-page.component';
import { NotFoundPageComponent } from './Shared/Pages/not-found-page/not-found-page.component';
import { HomePageComponent } from './Shared/Pages/home-page/home-page.component';


const routes: Routes = [
  {
    path: '',
    component: HomePageComponent
  },
  {
    path: 'monitor/availability',
    component: AvailabilityMonitorPageComponent
  },
  {
    path: 'monitor/availability/:ignoreFullscreen',
    component: AvailabilityMonitorPageComponent
  },
  {
    path: '**',
    component: NotFoundPageComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
