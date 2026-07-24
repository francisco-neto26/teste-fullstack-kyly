import { Component, EventEmitter, Input, Output, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-barra-busca',
  standalone: true,
  imports: [FormsModule, ButtonModule, InputTextModule],
  templateUrl: './barra-busca.component.html',
  styleUrl: './barra-busca.component.css'
})
export class BarraBuscaComponent implements OnInit, OnDestroy {
  termo = '';
  historico: string[] = [];

  @ViewChild('campoInput') campoInput?: ElementRef<HTMLInputElement>;

  private searchSubject = new Subject<string>();
  private searchSubscription?: Subscription;

  @Input() desabilitado = false;

  @Output() buscar = new EventEmitter<string>();

  ngOnInit(): void {
    this.carregarHistorico();

    // Debounce configurado
    this.searchSubscription = this.searchSubject.pipe(
      debounceTime(600),
      distinctUntilChanged()
    ).subscribe(termo => {
      this.executarBusca(termo);
    });
  }

  ngOnDestroy(): void {
    this.searchSubscription?.unsubscribe();
  }

  onInput(): void {
    this.searchSubject.next(this.termo.trim());
  }

  pesquisar(): void {
    this.executarBusca(this.termo.trim());
  }

  selecionarHistorico(termoHistorico: string): void {
    this.termo = termoHistorico;
    this.executarBusca(termoHistorico);
    setTimeout(() => this.campoInput?.nativeElement.focus(), 0);
  }

  limparHistorico(): void {
    this.historico = [];
    localStorage.removeItem('kyly_historico_buscas');
    this.campoInput?.nativeElement.focus();
  }

  private executarBusca(termo: string): void {
    // VALIDAÇÃO: Se o campo estiver vazio ou com menos de 2 caracteres, cancela e não realiza busca
    if (!termo || termo.length < 2) {
      return;
    }

    this.salvarHistorico(termo);
    this.buscar.emit(termo);
  }

  private salvarHistorico(termo: string): void {
    const filtrado = this.historico.filter(h => h.toLowerCase() !== termo.toLowerCase());
    this.historico = [termo, ...filtrado].slice(0, 5);
    localStorage.setItem('kyly_historico_buscas', JSON.stringify(this.historico));
  }

  private carregarHistorico(): void {
    const salvo = localStorage.getItem('kyly_historico_buscas');
    if (salvo) {
      try {
        this.historico = JSON.parse(salvo);
      } catch {
        this.historico = [];
      }
    }
  }
}
