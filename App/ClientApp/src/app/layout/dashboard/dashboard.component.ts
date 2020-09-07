import { Component, OnInit, Injector } from '@angular/core';
import { ScheduleLogService } from '../logs/schedulelog.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  public DashboardData: any = {};
  constructor(private injector: Injector) { }

  ngOnInit() {
    this.getDashboardData();
  }

  async getDashboardData() {
    let scheduleLogService = this.injector.get(ScheduleLogService);
    this.DashboardData = await scheduleLogService.GetDashboardData();
  }

}
