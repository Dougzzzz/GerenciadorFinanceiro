import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ContasListComponent } from './contas-list.component';
import { By } from '@angular/platform-browser';

describe('ContasListComponent', () => {
  let component: ContasListComponent;
  let fixture: ComponentFixture<ContasListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContasListComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(ContasListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render account list', () => {
    component.contas = [{ id: '1', nomeBanco: 'Nubank', saldoAtual: 500, provedor: 2 } as any];
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.item')).toBeTruthy();
    expect(compiled.querySelector('strong')?.textContent).toContain('Nubank');
  });

  it('should emit onIniciarEdicao when edit button is clicked', () => {
    spyOn(component.onIniciarEdicao, 'emit');
    const mockConta = { id: '1', nomeBanco: 'Nubank' } as any;
    component.contas = [mockConta];
    fixture.detectChanges();
    
    const editBtn = fixture.debugElement.query(By.css('.btn-icon'));
    editBtn.nativeElement.click();
    
    expect(component.onIniciarEdicao.emit).toHaveBeenCalledWith(mockConta);
  });
});
