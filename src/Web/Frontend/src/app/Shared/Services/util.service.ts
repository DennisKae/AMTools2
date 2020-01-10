import { Injectable, isDevMode } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UtilService {

  constructor() { }

  isNullOrUndefined(input: any): boolean {
    return input === undefined ||
      input === null;
  }

  isNullOrWhitespace(input: any): boolean {
    return this.isNullOrUndefined(input) ||
      input === '' ||
      (typeof (input) === "string" && input.trim() === '');
  }

  getAbsoluteUrlFromRelativeUrl(url: string) {
    if (this.isNullOrWhitespace(url)) {
      return url;
    }

    if (url.toLocaleLowerCase().startsWith("http")) {
      return url;
    }

    if (!url.startsWith("/")) {
      url = "/" + url;
    }

    if (isDevMode()) {
      url = "http://localhost:1112" + url;
    }

    return url;
  }
}
