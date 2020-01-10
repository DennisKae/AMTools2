import { Injectable, isDevMode } from '@angular/core';
import { HttpClient, HttpRequest, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UtilService } from './util.service';
import { ToastService } from './toast.service';

@Injectable({
  providedIn: 'root'
})
export class BaseHttpService {

  constructor(
    private httpClient: HttpClient,
    private util: UtilService,
    private toastService: ToastService) {
  }

  get<T>(url: string, options?: any): Observable<T> {
    // return this.httpClient.get<T>(this.getAbsoluteUrlFromRelativeUrl(url));

    return this.request<T>("GET", url, options);
  }

  post<T>(url: string, options?: any): Observable<T> {
    return this.request<T>("POST", url, options);
  }

  put<T>(url: string, options?: any): Observable<T> {
    return this.request<T>("PUT", url, options);
  }

  delete<T>(url: string, options?: any): Observable<T> {
    return this.request<T>("DELETE", url, options);
  }

  private request<T>(method: string, url: string, options?: any): Observable<T> {

    return Observable.create((observer: any) => {
      this.httpClient.request(new HttpRequest(method, this.util.getAbsoluteUrlFromRelativeUrl(url), options))
        .subscribe(
          (response) => {
            if (response.type === 0) {
              return;
            }
            const formattedResponse = response as HttpResponse<T>;

            if (!this.util.isNullOrUndefined(formattedResponse)) {
              observer.next(formattedResponse.body);
              observer.complete();
            }
          },
          (error) => {
            switch (error.status) {
              case 403:
                observer.complete();
                break;
              default:

                let toastTitle = "HTTP " + error.status + ": " + error.statusText;
                let toastMessage = error.Message;

                switch (error.status) {
                  case 0:
                    toastTitle = "Fehler";
                    toastMessage = "Kommunikationsfehler: Ist das Backend erreichbar?";
                    break;
                  case 404:
                    toastMessage = "Die angefragte URL wurde nicht gefunden.";
                    break;
                  default:
                    break;
                }

                if (typeof (error.error) === "string" && !this.util.isNullOrWhitespace(error.error)) {
                  toastMessage = error.error;
                }

                this.toastService.error(toastMessage, toastTitle);

                observer.error(error);
                observer.complete();
                break;
            }
          });
    });
  }
}
