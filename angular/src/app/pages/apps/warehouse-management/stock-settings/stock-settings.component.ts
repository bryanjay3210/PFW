import { AfterViewInit, Component, Input, OnInit } from '@angular/core';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldDefaultOptions } from '@angular/material/form-field';
import { UntypedFormControl } from '@angular/forms';
import { UntilDestroy } from '@ngneat/until-destroy';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { AlertService } from 'src/services/alert.service';
import { Router } from '@angular/router';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { StockSettings, User } from 'src/services/interfaces/models';
import { StockSettingsService } from 'src/services/stocksettings.service';

@UntilDestroy()
@Component({
  selector: 'vex-stock-settings',
  templateUrl: './stock-settings.component.html',
  styleUrls: ['./stock-settings.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ],
  providers: [
    {
      provide: MAT_FORM_FIELD_DEFAULT_OPTIONS,
      useValue: {
        appearance: 'standard'
      } as MatFormFieldDefaultOptions
    }
  ]
})

export class StockSettingsComponent implements OnInit, AfterViewInit {
  @Input()
  imageDefault = "assets/img/pfw_logo_sm.png";
  layoutCtrl = new UntypedFormControl('fullwidth');
  
  stockSettings: StockSettings;
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  constructor(
    private router: Router,
    private stockSettingsService: StockSettingsService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.StockSettings);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  ngOnInit() {
    if (this.currentUser === undefined || this.access === UserPermission.NoAccess) {
      this.alertService.unauthorizedNotification();
      this.router.navigate(['/login']);
      return;
    }

    this.getData();
  }

  ngAfterViewInit() {
  }

  getData() {
    this.stockSettingsService.getStockSettings().subscribe(result => {
      if (result) {
        this.stockSettings = result;
      }
    });
  }

  setCACAProduct(event: any) {
    this.stockSettings.isCaliforniaCAProduct = event.checked;
  }

  setCANVProduct(event: any) {
    this.stockSettings.isCaliforniaNVProduct = event.checked;
  }

  setNVCAProduct(event: any) {
    this.stockSettings.isNevadaCAProduct = event.checked;
  }

  setNVNVProduct(event: any) {
    this.stockSettings.isNevadaNVProduct = event.checked;
  }

  setDSCAProduct(event: any) {
    this.stockSettings.isDropShipCAProduct = event.checked;
  }

  setDSNVProduct(event: any) {
    this.stockSettings.isDropShipNVProduct = event.checked;
  }

  saveStockSettings() {
    this.stockSettingsService.updateStockSettings(this.stockSettings).subscribe(result => {
      if (result) {
        this.alertService.successNotification('Stock Settings', 'Save');
      }
      else {
        this.alertService.failNotification('Stock Settings', 'Save');
      }
    });
  }
}