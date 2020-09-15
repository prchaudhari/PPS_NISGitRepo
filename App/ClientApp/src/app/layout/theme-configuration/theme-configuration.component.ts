import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-theme-configuration',
  templateUrl: './theme-configuration.component.html',
  styleUrls: ['./theme-configuration.component.scss']
})
export class ThemeConfigurationComponent implements OnInit {

  public isDefault: boolean = true;
  public isCustome: boolean = false;
  isDefaultClicked() {
    this.isDefault = true;
    this.isCustome = false;
  }
  isCustomeClicked() {
    this.isDefault = false;
    this.isCustome = true;
  }
  public isTheme1Active: boolean = false;
  public isTheme2Active: boolean = false;
  public isTheme3Active: boolean = false;
  public isTheme4Active: boolean = false;
  public isTheme5Active: boolean = false;
  public isTheme0Active: boolean = true;
  //Functions call to click the theme of the page--
  theme1() {
    const dom: any = document.querySelector('body');
    dom.classList.add('theme1');
    dom.classList.remove('theme2');
    dom.classList.remove('theme3');
    dom.classList.remove('theme4');
    dom.classList.remove('theme5');
    dom.classList.remove('theme0');
    this.isTheme1Active = true;
    this.isTheme2Active = false;
    this.isTheme3Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
  }
  theme2() {
    const dom: any = document.querySelector('body');
    dom.classList.add('theme2');
    dom.classList.remove('theme1');
    dom.classList.remove('theme3');
    dom.classList.remove('theme4');
    dom.classList.remove('theme5');
    dom.classList.remove('theme0');
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = true;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
  }
  theme3() {
    const dom: any = document.querySelector('body');
    dom.classList.remove('theme2');
    dom.classList.remove('theme1');
    dom.classList.add('theme3');
    dom.classList.remove('theme4');
    dom.classList.remove('theme5');
    dom.classList.remove('theme0');
    this.isTheme1Active = false;
    this.isTheme3Active = true;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;

  }
  theme4() {
    const dom: any = document.querySelector('body');
    dom.classList.remove('theme2');
    dom.classList.remove('theme1');
    dom.classList.remove('theme3');
    dom.classList.add('theme4');
    dom.classList.remove('theme5');
    dom.classList.remove('theme0');
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = true;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
  }
  theme5() {
    const dom: any = document.querySelector('body');
    dom.classList.remove('theme2');
    dom.classList.remove('theme1');
    dom.classList.remove('theme3');
    dom.classList.remove('theme4');
    dom.classList.add('theme5');
    dom.classList.remove('theme0');
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = true;
    this.isTheme0Active = false;
  }
  theme0() {
    const dom: any = document.querySelector('body');
    dom.classList.remove('theme2');
    dom.classList.remove('theme1');
    dom.classList.remove('theme3');
    dom.classList.remove('theme4');
    dom.classList.remove('theme5');
    dom.classList.add('theme0');
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = true;
  }
  constructor() { }

  ngOnInit() {
  }

}
