import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';

@Injectable()
export class SharedService {
  private messageSource = new BehaviorSubject('');
  currentMessage = this.messageSource.asObservable();
  
  private _triggerAction = new Subject<void>();

  changeMessage(message: string) {
    this.messageSource.next(message);
  }

  public updateCountTrigger() {
    this._triggerAction.next();
  }

  public updateCountTriggered$(): Observable<void> {
    return this._triggerAction.asObservable();
  }
}