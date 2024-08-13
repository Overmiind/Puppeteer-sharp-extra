var p = Object.defineProperty;
var d = (s,a,t)=>a in s ? p(s, a, {
    enumerable: !0,
    configurable: !0,
    writable: !0,
    value: t
}) : s[a] = t;
var e = (s,a,t)=>(d(s, typeof a != "symbol" ? a + "" : a, t),
    t);
import {a as n} from "./axios.262c1ca8.js";
import {M as m} from "./MapeadorDeCamposDeResponse.01121813.js";
import {d as u} from "./dayjs.min.b82cae86.js";
class r {
    constructor(a) {
        e(this, "cpf_cnpj");
        e(this, "nome_receita");
        e(this, "nome_mae_receita");
        e(this, "nome_fantasia_receita");
        e(this, "data_nascimento_receita");
        e(this, "situacao");
        e(this, "tipo_pessoa");
        e(this, "ano_obito");
        m.mapear(this, a)
    }
    dataNascimentoFormatada() {
        return this.data_nascimento_receita != null ? u(this.data_nascimento_receita).format("DD/MM/YYYY") : void 0
    }
    isFalecida() {
        return this.ano_obito != null && this.ano_obito > 0
    }
    temSituacaoDefinida() {
        return this.situacao != null
    }
}
class i extends Error {
}
class w {
    static async buscarCpfCnpj(a, t) {
        return t != null ? await this.buscaExternaCpfCnpj(a, t) : {
            resposta: await this.buscaInternaCpfCnpj(a)
        }
    }
    static async buscaExternaCpfCnpj(a, t) {
        try {
            const {data: o, headers: c} = await n.get(`/pessoas/${a}/externo?captcha=${t}`);
            return {
                codigo_token: c["x-cnc-tkse"],
                resposta: new r(o)
            }
        } catch (o) {
            throw o.response.data != null && o.response.data.erro == "ErroNaConsultaReceita" ? new i(o.response.data.mensagem) : o
        }
    }
    static async buscaInternaCpfCnpj(a) {
        const {data: t} = await n.get(`/pessoas/${a}`);
        if (t.cpf_cnpj != null)
            return new r(t);
        throw new i("N\xE3o foram encontrados dados para este CPF/CNPJ")
    }
}
export {w as C, i as E, r as P};
