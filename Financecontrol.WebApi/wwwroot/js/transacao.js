async function carregarCartoes(contaId) {
    debugger;
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
        var cartaoSelecionado = cartaoSelect.parentElement.getAttribute('data-cartao-id');

        itens.forEach(function (c) {
            var opt = document.createElement('option');
            opt.value = c.id;
            opt.textContent = c.nome;
            cartaoSelect.appendChild(opt);

            if (cartaoSelecionado && cartaoSelecionado == c.id) {
                opt.selected = true;
            }
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

    initDateTimeSplitField();
}

function toggleTransacao() {
    var tipoTransacao = document.getElementById('Tipo');
    var tipoOperacao = document.getElementById('tipooperacao-group');

    tipoOperacao.style.display = tipoTransacao.value == 2 ? '' : 'none';

    toggleCartao();
}

function toggleCartao() {
    var tipoTransacao = document.getElementById('Tipo');
    var cartaoGroup = document.getElementById('cartao-group');
    var cartaoSelect = document.getElementById('CartaoId');

    var isDespesa = tipoTransacao.value == 2 /* Despesa */;
    cartaoGroup.style.display = isDespesa ? '' : 'none';
    cartaoSelect.disabled = !isDespesa;
}

function initDateTimeSplitField() {
    const dateInput = document.getElementById('DataEfetivacaoDate');
    const timeInput = document.getElementById('DataEfetivacaoTime');
    const hidden = document.getElementById('DataEfetivacao');

    function syncToHidden() {
        const date = dateInput.value; // yyyy-MM-dd
        const time = timeInput.value; // HH:mm
        if (date && time) {
            hidden.value = `${date}T${time}`;
        } else if (date && !time) {
            hidden.value = `${date}T00:00`;
        } else {
            hidden.value = '';
        }
    }

    dateInput.addEventListener('change', syncToHidden);
    timeInput.addEventListener('change', syncToHidden);

    // garante que sincroniza antes de enviar
    const form = hidden.closest('form');
    if (form) {
        form.addEventListener('submit', syncToHidden);
    }

    // já sincroniza na carga inicial
    syncToHidden();
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
