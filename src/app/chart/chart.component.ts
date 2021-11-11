import { Component, OnInit } from '@angular/core';
import * as Highcharts from 'highcharts';

@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.scss'],
})
export class ChartComponent implements OnInit {
  highcharts = Highcharts;
  chartOptions = {
    title: {
      text: '',
      style: {
        display: 'none',
      },
    },
    subtitle: {
      text: '',
      style: {
        display: 'none',
      },
    },
    chart: {
      type: 'areaspline',
      zoomType: 'x',
      backgroundColor: null,
    },

    xAxis: {
      categories: ['Always', 'Often', 'Meets', 'Below'],
      visible: false,
    },

    yAxis: {
      visible: false,
    },

    tooltip: {
      pointFormat: '{point.label} has <b>{point.y:,.0f}% </b> of employees',
    },
    plotOptions: {
      area: {
        startPoint: 'Always',
        marker: {
          enabled: false,
          symbol: 'circle',
          radius: 2,
          states: {
            hover: {
              enabled: true,
            },
          },
        },
      },
    },
    series: [
      {
        data: [17.5, 37.5, 45.0, 0],
      },
    ],
    credits: {
      enabled: false,
    },
    legend: { enabled: false },
  };

  constructor() {}

  ngOnInit(): void {}
}
