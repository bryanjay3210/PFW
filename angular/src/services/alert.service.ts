import { Injectable } from '@angular/core';
import Swal, { SweetAlertOptions, SweetAlertResult } from 'sweetalert2';

@Injectable({
  providedIn: 'root',
})
export class AlertService {
  private options: SweetAlertOptions = {};

  constructor() {
  }

  initializeOptions(): SweetAlertOptions {
    let options: SweetAlertOptions = {};
    options.allowOutsideClick = false;
    options.allowEscapeKey = false;
    options.showDenyButton = false;
    return options;
  }

  confirmRGAQuantities(message: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Confirm create RGA';
    this.options.html = message;
    this.options.icon = 'question';
    this.options.showDenyButton = false;
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Yes';
    this.options.cancelButtonText = 'No';
    return Swal.fire(this.options);
  }

  copyExistNotification(entityName: string) : Promise<SweetAlertResult> {
    let options: SweetAlertOptions = {};
    options.allowOutsideClick = false;
    options.allowEscapeKey = false;
    options.showDenyButton = false;
    options.title = entityName;
    options.text = entityName + ' already exists!';
    options.icon = 'info';
    options.showDenyButton = false;
    options.showCancelButton = false;
    options.showConfirmButton = true;
    options.confirmButtonText = 'Ok';
    return Swal.fire(options);
  }

  selectCopyType() : Promise<SweetAlertResult> {
    const inputOptions = { 'CAPA': 'CAPA', 'NSF' : 'NSF', 'DSC': 'DSC', 'OE' : 'OE' }
    let options: SweetAlertOptions = {};
    options.allowOutsideClick = false;
    options.allowEscapeKey = false;
    options.showDenyButton = false;
    
    options.title = 'Select Certified Item Type';
    options.text = ''
    options.input = 'radio';
    options.inputOptions = inputOptions;
    options.inputValidator = (value) => {
      return new Promise((resolve) => {
        if (value) {
          resolve('');
        } else {
          resolve('Please select a certified item type!');
        }
      });
     };
    
    options.showConfirmButton = true;
    options.showCancelButton = true;
    options.confirmButtonText = 'Select';
    options.cancelButtonText = 'Cancel';
    return Swal.fire(options);
  }
  
  showAllButtons(message: string) : Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Confirm Action';
    this.options.text = 'Flag ' + message + ' as?';
    this.options.icon = 'question';

    //this.options.denyButtonColor = 'red';
    //this.options.confirmButtonColor = 'purple';
    // this.options.cancelButtonColor = 'lightgray';
    this.options.showDenyButton = true;
    this.options.showConfirmButton = true;
    this.options.showCancelButton = true;

    // this.options.showCloseButton = true;
    // this.options.showLoaderOnConfirm = true;
    // this.options.showLoaderOnDeny = true;

    this.options.denyButtonText = 'Out Of Stock';
    this.options.confirmButtonText = 'Customer Cancelled';
    this.options.cancelButtonText = 'Cancel';

    return Swal.fire(this.options);
  }

  showBlockUI(message: string) {
    this.options = this.initializeOptions();
    this.options.title = 'Please Wait...';
    this.options.text = message;
    this.options.allowEscapeKey = false;
    this.options.allowOutsideClick = false;
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    //this.options.customClass = 'block-ui';
    Swal.fire(this.options);
    //Swal.update(this.options);
    Swal.showLoading();
  }

  showSessionTimeout(title: string, message: string, countdown: number): Promise<SweetAlertResult>  {
    this.options = this.initializeOptions();
    this.options.title = title;
    this.options.html = message;
    this.options.allowEscapeKey = false;
    this.options.allowOutsideClick = false;
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Continue';
    this.options.timer = countdown * 1000;
    this.options.timerProgressBar = true;
    return Swal.fire(this.options);
  }

  hideBlockUI() {
    Swal.hideLoading();
    Swal.close();
  }

  selectInactiveEntityNotification(entityName: string, key: any): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Invalid ' + entityName + ' Selection';
    this.options.text = 'Unable to select deactivated ' + entityName + ' ' + key + '. Please reactivate in ' + entityName + 's Module to proceed.' ;
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Ok';
    this.options.cancelButtonText = '';
    return Swal.fire(this.options);
  }

  selectAccountOnHoldNotification(entityName: string, key: any): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Invalid ' + entityName + ' Selection';
    this.options.text = 'Unable to select On-Hold ' + entityName + ' ' + key + '. Please uncheck Account On-Hold in ' + entityName + 's Module to proceed.' ;
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Ok';
    this.options.cancelButtonText = '';
    return Swal.fire(this.options);
  }

  printAllNotification(): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Confirm Print All Statements';
    this.options.text = 'This would take longer than the usual print time. Are you sure you want to print all statements?';
    this.options.icon = 'question';
    this.options.showDenyButton = false;
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Yes';
    this.options.cancelButtonText = 'No';
    return Swal.fire(this.options);
  }

  emailOrderConfirmation(type: string, orderNumber: number, email: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Confirm ' + type + ' Email';
    this.options.text = 'Are you sure you want to email the '+ type + ': ' + orderNumber + ' to ' + email + '?';
    this.options.icon = 'question';
    this.options.showDenyButton = false;
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Yes';
    this.options.cancelButtonText = 'No';
    return Swal.fire(this.options);
  }

  printInvoiceNotification() {
    this.options = this.initializeOptions();
    this.options.title = 'Please Wait...';
    this.options.text = 'Printing Invoice';
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }

  existNotification(entityName: string) {
    this.options = this.initializeOptions();
    this.options.title = entityName + ' Exist!';
    this.options.text = entityName + ' already exists in the list.';
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }

  successNotification(entityName: string, mode: string) {
    this.options = this.initializeOptions();
    this.options.title = mode + ' ' + entityName;
    this.options.text = entityName + ' Successfully ' + mode + 'd.';
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }

  successEmailNotification() {
    this.options = this.initializeOptions();
    this.options.title = 'Sending Email';
    this.options.text = 'Email(s) Successfully Sent.';
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }

  sendingEmailNotification() {
    this.options = this.initializeOptions();
    this.options.title = 'Sending Email';
    this.options.text = 'Email(s) are being Sent.';
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }

  failedEmailNotification() {
    this.options = this.initializeOptions();
    this.options.title = 'Sending Email Failed';
    this.options.text = 'An error was encountered while sending the email.';
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }

  failNotification(entityName: string, mode: string) {
    this.options = this.initializeOptions();
    this.options.title = mode + ' ' + entityName;
    this.options.text = mode + ' ' + entityName + ' failed.';
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }

  negativeValueNotification(message: string) {
    this.options = this.initializeOptions();
    this.options.title = 'Negative Value';
    this.options.text = message;
    this.options.icon = 'warning';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }

  errorNotification(message: string) {
    this.options = this.initializeOptions();
    this.options.title = 'Error';
    this.options.text = message;
    this.options.icon = 'error';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }

  noStockInLocationNotification(location: string) {
    this.options = this.initializeOptions();
    this.options.title = 'Get Location Stocks';
    this.options.text = 'There were no stocks found in location ' + location;
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }

  failOnLocationNotification(entityName: string, productNo: string, location: string) {
    this.options = this.initializeOptions();
    this.options.title = 'Search ' + entityName;
    this.options.text = entityName + ' ' + productNo + ' does not exist in location ' + location;
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }

  failCreateNotification(entityName: string, mode: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = mode + ' ' + entityName+ ' failed.';
    this.options.text = 'Would you like to create the ' + entityName + '?';
    this.options.icon = 'question';
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Yes';
    this.options.cancelButtonText = 'No';
    return Swal.fire(this.options);
  }

  failNotificationSearchDriverLogOrder(entityName: string, mode: string) {
    this.options = this.initializeOptions();
    this.options.title = mode + ' ' + entityName;
    this.options.text = entityName + ' not found or is already delivered.';
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }

  foundRecordNotification(entityName: string) {
    this.options = this.initializeOptions();
    this.options.title = entityName + ' Found!';
    this.options.text = entityName + ' record fetched.';
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }

  notFoundRecordNotification(entityName: string) {
    this.options = this.initializeOptions();
    this.options.title = entityName + ' Not Found!';
    this.options.text = entityName + ' record not fetched.';
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }

  notFoundNotification(entityName: string) {
    this.options = this.initializeOptions();
    this.options.title = entityName + ' Not Found!';
    this.options.text = entityName + ' record not found.';
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }

  zeroRecordNotification(entityName: string) {
    this.options = this.initializeOptions();
    this.options.title = 'Fetch ' + entityName + ' Records';
    this.options.text = 'Zero(0) ' + entityName + ' fetched.';
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }

  voidNotification(entityName: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Confirm Void ' + entityName;
    this.options.text = 'Are you sure you want to void the ' + entityName + '?';
    this.options.icon = 'question';
    this.options.showDenyButton = false;
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Yes';
    this.options.cancelButtonText = 'No';
    return Swal.fire(this.options);
  }

  printSuccessNotification(entityName: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Please Check';
    this.options.text = 'Did the ' + entityName + '(s) printed successfully?';
    this.options.icon = 'question';
    this.options.showDenyButton = false;
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Yes';
    this.options.cancelButtonText = 'No';
    return Swal.fire(this.options);
  }

  cancelNotification(entityName: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Confirm ' + entityName + ' Update Cancel';
    this.options.text = 'Are you sure you want to cancel the ' + entityName + ' update?';
    this.options.icon = 'question';
    this.options.showDenyButton = false;
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Yes';
    this.options.cancelButtonText = 'No';
    return Swal.fire(this.options);
  }

  createNotification(entityName: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Confirm create ' + entityName;
    this.options.text = 'Are you sure you want to create the ' + entityName + '?';
    this.options.icon = 'question';
    this.options.showDenyButton = false;
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Yes';
    this.options.cancelButtonText = 'No';
    return Swal.fire(this.options);
  }

  createOverpaymentNotification(): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Confirm create Payment';
    this.options.text = 'Your payment amount is greater than the total amount due! This would post an Overpayment record in Order Management. Are you sure you want to proceed with the Payment?';
    this.options.icon = 'question';
    this.options.showDenyButton = false;
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Yes';
    this.options.cancelButtonText = 'No';
    return Swal.fire(this.options);
  }

  createUnderpaymentNotification(): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Confirm create Payment';
    this.options.text = 'Your payment amount is less than the total amount due! Are you sure you want to proceed with the Payment?';
    this.options.icon = 'question';
    this.options.showDenyButton = false;
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Yes';
    this.options.cancelButtonText = 'No';
    return Swal.fire(this.options);
  }

  partsTransferNotification(source: string, destination: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Confirm Parts Transfer';
    this.options.text = 'Are you sure you want to transfer the parts from location ' + source + ' to ' + destination + '?';
    this.options.icon = 'question';
    this.options.showDenyButton = false;
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Transfer';
    this.options.cancelButtonText = 'Cancel';
    return Swal.fire(this.options);
  }

  updateNotification(entityName: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Confirm update ' + entityName;
    this.options.text = 'Are you sure you want to update the ' + entityName + '?';
    this.options.icon = 'question';
    this.options.showDenyButton = false;
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Update';
    this.options.cancelButtonText = 'Cancel';
    return Swal.fire(this.options);
  }

  actionNotificationV2( action: string, entityName: string, message: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Confirm ' + action + ' ' + entityName;
    this.options.text = 'Are you sure you want to ' + action + ' the ' + entityName + ' ' + message + '?';
    this.options.icon = 'question';
    this.options.showDenyButton = false;
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = action;
    this.options.cancelButtonText = 'Cancel';
    return Swal.fire(this.options);
  }

  actionNotification( action: string, entityName: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Confirm ' + action + ' ' + entityName;
    this.options.text = 'Are you sure you want to ' + action + ' the ' + entityName + '?';
    this.options.icon = 'question';
    this.options.showDenyButton = false;
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = action;
    this.options.cancelButtonText = 'Cancel';
    return Swal.fire(this.options);
  }

  deleteNotification(entityName: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Confirm delete ' + entityName;
    this.options.text = 'Are you sure you want to delete the ' + entityName + '?';
    this.options.icon = 'question';
    this.options.showDenyButton = false;
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Delete';
    this.options.cancelButtonText = 'Cancel';
    return Swal.fire(this.options);
  }

  removeNotification(entityName: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Confirm Remove ' + entityName;
    this.options.text = 'Are you sure you want to remove the selected ' + entityName + '?';
    this.options.icon = 'question';
    this.options.showDenyButton = false;
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Remove';
    this.options.cancelButtonText = 'Cancel';
    return Swal.fire(this.options);
  }

  clearNotification(entityName: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Confirm clear ' + entityName;
    this.options.text = 'Are you sure you want to clear the ' + entityName + '?';
    this.options.icon = 'question';
    this.options.showDenyButton = false;
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Clear';
    this.options.cancelButtonText = 'Cancel';
    return Swal.fire(this.options);
  }

  warningConfirmationNotification(message: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Warning';
    this.options.text = message +  '\n Are you sure you want to proceed?';
    this.options.icon = 'question';
    this.options.showDenyButton = false;
    this.options.showCancelButton = true;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Yes';
    this.options.cancelButtonText = 'No';
    return Swal.fire(this.options);
  }

  validationNotification(entityName: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Record Validation Failed!';
    this.options.text = 'Unable to save. There are failed validations on the ' + entityName + ' record.';
    this.options.icon = 'warning';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Ok';
    return Swal.fire(this.options);
  }

  validationRequiredNotification(message: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Required Field Validation';
    this.options.text = message;
    this.options.icon = 'warning';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Ok';
    return Swal.fire(this.options);
  }

  requiredNotification(message: string){
    this.options = this.initializeOptions();
    this.options.title = 'Required Field Validation';
    this.options.text = message;
    this.options.icon = 'info';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }

  validationFailedNotification(title: string, message: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = title;
    this.options.text = message;
    this.options.icon = 'warning';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Close';
    return Swal.fire(this.options);
  }

  duplicateNotification(entityName: string): Promise<SweetAlertResult> {
    this.options = this.initializeOptions();
    this.options.title = 'Record Validation Failed!';
    this.options.text = 'Unable to save. Duplicate values detected on the ' + entityName + ' record.';
    this.options.icon = 'warning';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = true;
    this.options.confirmButtonText = 'Ok';
    return Swal.fire(this.options);
  }

  unauthorizedNotification() {
    this.options = this.initializeOptions();
    this.options.title = 'Authorization Failed!';
    this.options.text = 'You are not authorized to access this site. Please login to access.';
    this.options.icon = 'error';
    this.options.showDenyButton = false;
    this.options.showCancelButton = false;
    this.options.showConfirmButton = false;
    this.options.confirmButtonText = 'Ok';
    Swal.fire(this.options);
    setTimeout(()=>{
      Swal.close();
    },3000); 
  }
  // successNotification(
  //   title: string,
  //   message: string
  // ): Promise<SweetAlertResult> {
  //   this.options.icon = 'info';
  //   this.options.title = title;
  //   this.options.text = message;
  //   return Swal.fire(this.options);
  // }

  tinyAlert() {
    Swal.fire('Hey there!');
  }

  alertConfirmation() {
    Swal.fire({
      title: 'Are you sure?',
      text: 'This process is irreversible.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, go ahead.',
      cancelButtonText: 'No, let me think',
    }).then((result) => {
      if (result.value) {
        Swal.fire('Removed!', 'Product removed successfully.', 'success');
      } else if (result.dismiss === Swal.DismissReason.cancel) {
        Swal.fire('Cancelled', 'Product still in our database.)', 'error');
      }
    });
  }
}
