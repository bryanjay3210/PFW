import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { QuicklinkModule } from 'ngx-quicklink';
import { VexRoutes } from 'src/@vex/interfaces/vex-route.interface';
import { AccountsReceivableTableComponent } from './accounts-receivable-table.component';


const routes: VexRoutes = [
  {
    path: '',
    component: AccountsReceivableTableComponent,
    data: {
      toolbarShadowEnabled: false
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule, QuicklinkModule]
})
export class AccountsReceivableTableRoutingModule {
}
