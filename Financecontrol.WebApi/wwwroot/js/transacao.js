async function carregarCartoes(contaId) {
    var cartaoSelect = document.getElementById('CartaoId');
    if (!cartaoSelect) return;

    cartaoSelect.innerHTML = '';
    var optDefault = document.createElement('option');
    optDefault.value = '';
    optDefault.textContent = 'Selecione...';
    cartaoSelect.appendChild(optDefault);

    if (!contaId) {
        toggleCartao();
        return;
    }

    try {
        var url = '/Transacao/CartoesPorConta?contaBancariaId=' + encodeURIComponent(contaId);
        var resp = await fetch(url, { headers: { 'Accept': 'application/json' } });
        if (!resp.ok) throw new Error('Falha ao carregar cartões');
        var itens = await resp.json();
        itens.forEach(function (c) {
            var opt = document.createElement('option');
            opt.value = c.id;
            opt.textContent = c.nome;
            cartaoSelect.appendChild(opt);
        });
    } catch (e) {
        console.error('Erro ao carregar cartões:', e);
    }

    toggleCartao();
}

function carregarDadosIniciais() {
    var contaSelect = document.getElementById('ContaBancariaId');
    if (contaSelect && contaSelect.value) {
        carregarCartoes(contaSelect.value);
    } else {
        toggleTransacao();
    }
}

function toggleTransacao() {
    var tipoTransacao = document.getElementById('Tipo');
    var tipoOperacaoGroup = document.getElementById('tipooperacao-group');

    if (!tipoTransacao || !tipoOperacaoGroup) return;

    tipoOperacaoGroup.style.display = 
        tipoTransacao.value == 1 /* Despesa */ ? '' : 'none';

    toggleCartao();
}

function toggleCartao() {
    var tipoOperacao = document.getElementById('TipoOperacao');
    var tipoTransacao = document.getElementById('Tipo');
    var cartaoGroup = document.getElementById('cartao-group');
    var cartaoSelect = document.getElementById('CartaoId');
    if (!tipoOperacao || !cartaoGroup || !cartaoSelect) return;

    var isCredito = tipoOperacao.value == 2 /* Crédito */ 
                 && tipoTransacao.value == 1 /* Despesa */;
    cartaoGroup.style.display = isCredito ? '' : 'none';
    cartaoSelect.disabled = !isCredito;
}

document.addEventListener('DOMContentLoaded', function () {
    var tipoOperacao = document.getElementById('TipoOperacao');
    var tipoTransacao = document.getElementById('Tipo');
    var contaSelect = document.getElementById('ContaBancariaId');

    if (tipoOperacao) tipoOperacao.addEventListener('change', toggleCartao);
    if (tipoTransacao) tipoTransacao.addEventListener('change', toggleTransacao);
    if (contaSelect) contaSelect.addEventListener('change', function () {
        carregarCartoes(this.value);
    });

    carregarDadosIniciais();
});
