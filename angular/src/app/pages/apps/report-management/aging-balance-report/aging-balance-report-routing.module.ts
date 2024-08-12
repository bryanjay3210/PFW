import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { QuicklinkModule } from 'ngx-quicklink';
import { VexRoutes } from 'src/@vex/interfaces/vex-route.interface';
import { AgingBalanceReportComponent } from './aging-balance-report.component';


const routes: VexRoutes = [
  {
    path: '',
    component: AgingBalanceReportComponent,
    data: {
      toolbarShadowEnabled: false
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule, QuicklinkModule]
})
export class AgingBalanceReportRoutingModule {
}
