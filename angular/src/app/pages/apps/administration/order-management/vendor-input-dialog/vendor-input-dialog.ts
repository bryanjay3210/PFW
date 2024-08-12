import { Component, ElementRef, Inject, ViewChild } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSelectChange } from '@angular/material/select';
import { Sort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { UntilDestroy } from '@ngneat/until-destroy';
import moment from 'moment';
import { Observable, map, startWith } from 'rxjs';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { AlertService } from 'src/services/alert.service';
import { OrderDetail, User, Vendor, VendorCatalog } from 'src/services/interfaces/models';
import { VendorService } from 'src/services/vendor.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';

@UntilDestroy()
@Component({
  selector: 'vex-vendor-input-dialog',
  templateUrl: './vendor-input-dialog.html',
  styleUrls: ['./vendor-input-dialog.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})

export class VendorInputDialog {
  @ViewChild('inputVendor', { static: false }) inputVendor: ElementRef;
  sortDataMatTable($event: Sort) {
    throw new Error('Method not implemented.');
  }

  vendorList: Vendor[];
  filteredVendors$: Observable<Vendor[]>;
  vendorCodeCtrl = new UntypedFormControl;
  vendorPartNumberCtrl = new UntypedFormControl;
  vendorPriceCtrl = new UntypedFormControl;
  vendorQuantityCtrl = new UntypedFormControl;

  state: string = 'CA';

  columns: TableColumn<VendorCatalog>[] = [
    { label: 'Vendor Code', property: 'vendorCode', type: 'text', visible: true },
    { label: 'Cutoff Time', property: 'cutoffTime', type: 'text', visible: true },
    { label: 'Part Number', property: 'vendorPartNumber', type: 'text', visible: true },
    { label: 'Price', property: 'price', type: 'text', visible: true },
    { label: 'Stock', property: 'onHand', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
  ];

  dataSource: MatTableDataSource<VendorCatalog> | null;
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: any,
    private vendorService: VendorService,
    private dialogRef: MatDialogRef<VendorInputDialog>,
    private alertService: AlertService
  ) {

    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.OrderManagement);
    this.access = modulePermission.accessTypeId;
  }

  ngOnInit() {
    this.dataSource = new MatTableDataSource();
    if (this.defaults.orderDetail) {
      if (this.defaults.state) {
        this.state = this.defaults.state;
      }
      this.dataSource.data = this.defaults.orderDetail.vendorCatalogs;
      this.vendorPartNumberCtrl.setValue(this.defaults.orderDetail.mainPartsLinkNumber);
      this.vendorPriceCtrl.setValue(0.00);
      this.vendorQuantityCtrl.setValue(1);
      this.getVendorList();
    }
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  getVendorList() {
    this.vendorService.getVendorsByState(this.state).subscribe(result => {
      if (result) {
        this.vendorList = result;
        this.initializeVendorList();
      }
    });
  }

  initializeVendorList() {
    this.vendorCodeCtrl = new UntypedFormControl();
    this.filteredVendors$ = this.vendorCodeCtrl.valueChanges.pipe(
      startWith(''),
      map(code => code ? this.filterVendors(code) : this.vendorList.slice())
    );
  }



  filterVendors(svendors: string): any {
    return this.vendorList.filter(e => e.vendorCode.toLowerCase().includes(svendors.toLowerCase()));
  }
  onCodeEnter(event) {
    if (event.value && event.value.length > 0) {
      let code = undefined;
      let vendorList = this.vendorList.filter(e => e.vendorCode.includes(event.value));
      if (vendorList.length === 1) {
        code = vendorList[0];
      }

      if (code) {
        this.vendorCodeCtrl.setValue(code);
        this.onVendorSelectionChange(code);
      }
      else {
        this.inputVendor.nativeElement.value = '';
        this.vendorCodeCtrl.setValue(undefined);
      }
    }
  }

  onVendorSelectionChange(code: string) {
    // this.selectedVendor = this.vendorList.find(c => c.vendorCode === code);
    // this.productFilterDTO.year = this.selectedYear.yearNumber;
    // this.initializeFilters(PartsFilter.Year);
  }

  resetVendorControl() {
    this.vendorCodeCtrl.reset();
  }

  setManualVendor() {
    if (!this.vendorCodeCtrl.value || this.vendorCodeCtrl.value.length === 0) {
      this.alertService.validationRequiredNotification('Vendor Code is required.')
      return;
    }
    
    if (!this.vendorPartNumberCtrl.value || this.vendorPartNumberCtrl.value.length === 0) {
      this.alertService.validationRequiredNotification('Vendor Part Number is required.')
      return;
    }

    if (!this.vendorPriceCtrl.value || Number(this.vendorPriceCtrl.value) === 0) {
      this.alertService.validationRequiredNotification('Vendor Price should be greater than zero.')
      return;
    }

    let vendor = { vendorCode : this.vendorCodeCtrl.value,  vendorPartNumber: this.vendorPartNumberCtrl.value, vendorPrice: this.vendorPriceCtrl.value, vendorQuantity: this.vendorQuantityCtrl.value }
    this.dialogRef.close(vendor);
  }

  onLabelChange(change: MatSelectChange, row: OrderDetail) {
    let vendor = { vendorCode : change.value.vendorCode,  vendorPartNumber: change.value.vendorPartNumber, vendorPrice: change.value.price, vendorQuantity: change.value.onHand }
    this.dialogRef.close(vendor);
  }

  rowClicked(row: VendorCatalog) {
    let vendor = { vendorCode : row.vendorCode,  vendorPartNumber: row.vendorPartNumber, vendorPrice: row.price, vendorQuantity: row.onHand }
    this.dialogRef.close(vendor);
  }

  formatTime(time: any) {
    if (!time) return '';
    return moment(time).format("hh:mm A")
  }
}