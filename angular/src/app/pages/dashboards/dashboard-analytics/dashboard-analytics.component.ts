import { Component, OnInit } from '@angular/core';
import { defaultChartOptions } from '../../../../@vex/utils/default-chart-options';
import { Order, tableSalesData } from '../../../../static-data/table-sales-data';
import { TableColumn } from '../../../../@vex/interfaces/table-column.interface';
import { BnNgIdleService } from 'bn-ng-idle';
import { AlertService } from 'src/services/alert.service';
import { Router } from '@angular/router';
import { UserService } from 'src/services/user.service';
import * as $ from "jquery";
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'vex-dashboard-analytics',
  templateUrl: './dashboard-analytics.component.html',
  styleUrls: ['./dashboard-analytics.component.scss']
})
export class DashboardAnalyticsComponent implements OnInit {
  imageDefaultPFW = 'assets/img/pfitwest_dashboard.jpg';
  imageDefaultPCO = 'assets/img/Parts Co LoGO_dashboard.jpg';

  constructor(private router: Router, private matDialog: MatDialog, private bnIdle: BnNgIdleService, private alertService: AlertService, private userService: UserService) {}

  ngOnInit(): void {
    this.userService.getSessionTimeout().subscribe(result => {
      if (result) {
        this.bnIdle.startWatching(result.sessionTimeout).subscribe((isTimedOut: boolean) => {
          if (isTimedOut) {
            let countdown = result.dialogCountdown;
            let closeInSeconds = countdown, displayText = "You have been logged out due to inactivity!<br>You will be redirected to the Landing page in<br><br><div style='font-weight: bolder; font-size: xx-large;'>#1<span> seconds.<span></div><br><br>Hit continue to resume session.", timer;

            this.alertService.showSessionTimeout('Session Timeout', displayText.replace(/#1/, closeInSeconds.toString()), countdown).then(answer => {
              if (answer.isConfirmed) { 
                //this.bnIdle.stopTimer();
                //this.bnIdle.resetTimer(result.sessionTimeout);
                closeInSeconds = -1;
                clearInterval(timer);
                this.alertService.hideBlockUI();
              }
              else {
                // closeInSeconds = -1;
                // clearInterval(timer);
                localStorage.setItem('CurrentUser', undefined);
                localStorage.setItem('CurrentUserRole', undefined);
                localStorage.setItem('Token', undefined);
                this.bnIdle.stopTimer();
                this.router.navigate(['landing']);
              }
            });

            timer = setInterval(function() {
              closeInSeconds--;
              if (closeInSeconds < 0) {
          
                  clearInterval(timer);
              }
              $('.swal2-html-container').html(displayText.replace(/#1/, closeInSeconds.toString()));
            }, 1000);
          }
        });
      }
    });
  }

  tableColumns: TableColumn<Order>[] = [
    {
      label: '',
      property: 'status',
      type: 'badge'
    },
    {
      label: 'PRODUCT',
      property: 'name',
      type: 'text'
    },
    {
      label: '$ PRICE',
      property: 'price',
      type: 'text',
      cssClasses: ['font-medium']
    },
    {
      label: 'DATE',
      property: 'timestamp',
      type: 'text',
      cssClasses: ['text-secondary']
    }
  ];
  tableData = tableSalesData;

  series: ApexAxisChartSeries = [{
    name: 'Subscribers',
    data: [28, 40, 36, 0, 52, 38, 60, 55, 67, 33, 89, 44]
  }];

  userSessionsSeries: ApexAxisChartSeries = [
    {
      name: 'Users',
      data: [10, 50, 26, 50, 38, 60, 50, 25, 61, 80, 40, 60]
    },
    {
      name: 'Sessions',
      data: [5, 21, 42, 70, 41, 20, 35, 50, 10, 15, 30, 50]
    },
  ];

  salesSeries: ApexAxisChartSeries = [{
    name: 'Sales',
    data: [28, 40, 36, 0, 52, 38, 60, 55, 99, 54, 38, 87]
  }];

  pageViewsSeries: ApexAxisChartSeries = [{
    name: 'Page Views',
    data: [405, 800, 200, 600, 105, 788, 600, 204]
  }];

  uniqueUsersSeries: ApexAxisChartSeries = [{
    name: 'Unique Users',
    data: [356, 806, 600, 754, 432, 854, 555, 1004]
  }];

  uniqueUsersOptions = defaultChartOptions({
    chart: {
      type: 'area',
      height: 100
    },
    colors: ['#ff9800']
  });

}
