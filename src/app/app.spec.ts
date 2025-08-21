import { provideZonelessChangeDetection } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';
import { routes } from './app.routes';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        provideZonelessChangeDetection(),
        provideRouter(routes)
      ]
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render title', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Hello, web');
  });

  it('should render congratulations message', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('p')?.textContent).toContain('Congratulations! Your app is running. 🎉');
  });

  it('should contain router outlet', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('router-outlet')).toBeTruthy();
  });

  it('should render navigation links', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const links = compiled.querySelectorAll('a.pill');
    expect(links.length).toBe(6); // Based on the template, there are 6 navigation links
  });

  it('should have correct link to Angular docs', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const docsLink = Array.from(compiled.querySelectorAll('a.pill')).find(
      link => link.textContent?.includes('Explore the Docs')
    ) as HTMLAnchorElement;
    expect(docsLink).toBeTruthy();
    expect(docsLink.href).toBe('https://angular.dev/');
  });

  it('should have external links with correct attributes', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const externalLinks = compiled.querySelectorAll('a[target="_blank"]');
    externalLinks.forEach(link => {
      expect(link.getAttribute('rel')).toBe('noopener');
    });
  });

  it('should have social media links', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const socialLinks = compiled.querySelectorAll('.social-links a');
    expect(socialLinks.length).toBeGreaterThan(0);
  });

  it('should have proper semantic structure', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    
    // Check for main content structure
    const main = compiled.querySelector('main');
    expect(main).toBeTruthy();
    
    // Check for divider with proper aria-label
    const divider = compiled.querySelector('.divider[role="separator"]');
    expect(divider).toBeTruthy();
    expect(divider?.getAttribute('aria-label')).toBe('Divider');
  });
});
