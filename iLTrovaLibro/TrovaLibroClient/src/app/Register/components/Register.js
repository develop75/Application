'use client';

import { useState, useEffect, useRef } from 'react';

import Link from 'next/link';
import { User, Mail, Lock, Phone, MapPin, Hash, Truck, Handshake, ChevronRight } from 'lucide-react';
import ReCAPTCHA from "react-google-recaptcha";

import ProvinceSelect from '@/components/ProvinceSelect'
import SearchableSelect from '@/components/SearchableSelect'
import InputGroup from '@/components/InputGroup'
import SelectionCard from '@/components/SelectionCard'

import { getProvinces } from '@/services/dictionary'



const Register = () => {

    const formRef = useRef(null);
    const provinceRef = useRef(null);
    const cityRef = useRef(null);


    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const [provinces, setProvinces] = useState([]); // Caricate da API
    const [availableCities, setAvailableCities] = useState([]);

    const [canShip, setCanShip] = useState(false)

    const [formData, setFormData] = useState({
        nome: '', cognome: '', cf: '', indirizzo: '', cap: '',
        provincia: '', citta: '', cellulare: '', mail: '', provinceId: '', cityId: '',
        password: '', shippingMethod: 'scambio' // 'spedizione' o 'scambio'
    });

    const handleChange = (e) => {
        const { name, value, originalItem } = e.target;

        setFormData(prev => ({ ...prev, [name]: value }));

        // Se cambia la provincia, popola le città
        if (name === 'provinceId') {
            setAvailableCities(originalItem.cities || []);
            setFormData(prev => ({ ...prev, cityId: '' })); // Resetta la città

            // reset stato invalid sulla città se aveva errore
            cityRef.current?.setInvalid(false);
        }
    };


    const onChange = (value) => {
        console.log("Captcha value:", value);
        // Qui puoi salvare il valore nello stato per inviarlo al backend
    };

    const handleSubmit = async (e) => {

        const provinceOk = provinceRef.current?.validate?.() ?? true;
        const cityOk = cityRef.current?.validate?.() ?? true;

        const nativeOk = formRef.current?.checkValidity?.() ?? tr


        if (!nativeOk || !provinceOk || !cityOk) {
            e.preventDefault();

            // Mostra i messaggi nativi per gli input standard
            formRef.current?.reportValidity?.();

            // Porta il focus sul primo custom select non valido
            if (!provinceOk) {
                provinceRef.current?.focus?.();
                provinceRef.current?.setInvalid?.(true); // se esposto
            } else if (!cityOk) {
                cityRef.current?.focus?.();
                cityRef.current?.setInvalid?.(true); // se esposto
            }

            console.log('Validazione fallita:', { nativeOk, provinceOk, cityOk, formData });
            return;
        }

        e.preventDefault();
        setLoading(true);
        setError(null);

        // Prepariamo il payload per l'API
        // Adattalo ai nomi delle proprietà che il tuo DTO C# si aspetta
        const payload = {
            ...formData,
            // Assicuriamoci che i valori numerici siano corretti
            provinceId: parseInt(formData.provinceId),
            cityId: parseInt(formData.cityId),
            // Aggiungi qui eventuali campi extra richiesti dal backend
        };

        console.log("handleSubmit", payload)

        try {
            const response = await fetch('https://localhost:7065/api/Account/Register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-App-Identify-Token': 'IlTuoCodiceSegretoPrivato' // L'header richiesto dal tuo middleware
                },
                body: JSON.stringify(payload),
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.detail || "Errore durante la registrazione");
            }

            // Successo!
            alert("Registrazione completata con successo!");
            router.push('/Login'); // Reindirizza al login

        } catch (err) {
            setError(err.message);
            console.error("Submit Error:", err);
        } finally {
            setLoading(false);
        }
    };


    useEffect(() => {

        const fetchProvinces = async () => {
            try {
                const data = await getProvinces()

                setProvinces(data);
            } catch (error) { console.error("Errore province:", error); }
        };

        fetchProvinces();

    }, []);

    return (
        <div className="min-h-screen bg-slate-50 pt-4 md:pt-6 pb-10">
            <div className="sm:mx-auto sm:w-full sm:max-w-3xl">
                <h2 className="text-center text-3xl font-black text-slate-800 tracking-tight">
                    Crea il tuo account
                </h2>
                <p className="mt-2 text-center text-sm text-slate-500 font-medium">
                    Unisciti alla community di iLTrovaLibro
                </p>
            </div>

            <div className="mt-10 sm:mx-auto sm:w-full sm:max-w-3xl">
                <div className="bg-white py-10 px-8 shadow-[0_20px_50px_rgba(0,0,0,0.05)] border border-slate-100 rounded-[3rem]">
                    <form ref={formRef} className="space-y-6" onSubmit={handleSubmit}>

                        {/* SEZIONE 1: ANAGRAFICA */}
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <InputGroup required label="Nome" name="nome" icon={<User size={18} />} placeholder="Mario" onChange={handleChange} />
                            <InputGroup required label="Cognome" name="cognome" icon={<User size={18} />} placeholder="Rossi" onChange={handleChange} />
                            {/*<InputGroup required label="Codice Fiscale *" name="cf" icon={<Hash size={18} />} placeholder="RSSMRA..." onChange={handleChange} />*/}
                            <InputGroup required label="Cellulare" name="cellulare" type="phone" icon={<Phone size={18} />} placeholder="+39 333..." onChange={handleChange} />
                        </div>

                        <hr className="border-slate-50" />

                        {/* SEZIONE 2: RESIDENZA */}
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <div className="md:col-span-2">
                                <InputGroup required label="Indirizzo" name="indirizzo" icon={<MapPin size={18} />} placeholder="Via Roma, 10" onChange={handleChange} />
                            </div>
                            {/*<InputGroup label="Città" name="citta" icon={<MapPin size={18} />} placeholder="Milano" onChange={handleChange} />*/}

                            {/* SELECT PROVINCE */}
                            <SearchableSelect
                                ref={provinceRef}
                                label="Provincia"
                                name="provinceId"
                                required
                                items={provinces}
                                value={formData.provinceId}
                                itemLabel="provinceName"
                                itemBadge="provinceCode"
                                onChange={handleChange}
                                icon={<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" /><path d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" /></svg>}
                            />

                            {/* SELECT CITTÀ */}
                            <SearchableSelect
                                ref={cityRef}
                                label="Città"
                                name="cityId"
                                required
                                items={availableCities}
                                value={formData.cityId}
                                itemLabel="cityName"
                                itemBadge="cityCode"
                                onChange={handleChange}
                                placeholder={formData.provinceId ? "Seleziona città..." : "Scegli prima una provincia"}
                                icon={<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" /></svg>}
                            />
                            <div className="grid grid-cols-2 gap-4">
                                <InputGroup required label="CAP" name="cap" placeholder="20100" onChange={handleChange} />
                                <InputGroup required label="Prov." name="provincia" placeholder="MI" onChange={handleChange} />
                            </div>
                        </div>

                        <hr className="border-slate-50" />

                        {/* SEZIONE 3: ACCOUNT */}
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <InputGroup required label="Mail" name="mail" icon={<Mail size={18} />} placeholder="mario88" onChange={handleChange} />
                            <InputGroup required label="Password" name="password" type="password" icon={<Lock size={18} />} placeholder="••••••••" onChange={handleChange} />
                        </div>

                        {/* SEZIONE 4: PREFERENZA CONSEGNA */}
                        <div className="space-y-4">
                            <label className="text-sm font-bold text-slate-700 ml-1">Preferenza Venditore</label>
                            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                                <SelectionCard
                                    active={formData.shippingMethod === 'spedizione'}
                                    onClick={() => {
                                        setCanShip(true)
                                        setFormData(p => ({ ...p, shippingMethod: 'spedizione' }))
                                    }}
                                    icon={<Truck size={24} />}
                                    title="Disponibile per Spedizione"
                                    desc="Posso spedire i libri con corriere"
                                />
                                <SelectionCard
                                    active={formData.shippingMethod === 'scambio'}
                                    onClick={() => {
                                        setCanShip(false)
                                        setFormData(p => ({ ...p, shippingMethod: 'scambio' }))
                                    }}
                                    icon={<Handshake size={24} />}
                                    title="Solo Scambio a mano"
                                    desc="Preferisco incontrare l'acquirente"
                                />
                            </div>

                            {/* Campo IBAN Condizionale */}
                            {/*{canShip && (*/}
                            {/*    <div className="space-y-2 animate-in fade-in slide-in-from-top-2 duration-300">*/}
                            {/*        <label className="text-xs font-bold text-slate-600 uppercase ml-1">*/}
                            {/*            Il tuo IBAN (per ricevere i pagamenti)*/}
                            {/*        </label>*/}
                            {/*        <div className="relative">*/}
                            {/*            <input*/}
                            {/*                type="text"*/}
                            {/*                placeholder="IT 00 X 00000 00000 000000000000"*/}
                            {/*                className="w-full p-3 bg-white border border-[#f17829] rounded-xl focus:outline-none focus:ring-2 focus:ring-[#f17829]/20 font-mono text-sm uppercase"*/}
                            {/*                required={canShip}*/}
                            {/*            />*/}
                            {/*            <div className="absolute right-3 top-3 text-[#f17829]">*/}
                            {/*                <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">*/}
                            {/*                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />*/}
                            {/*                </svg>*/}
                            {/*            </div>*/}
                            {/*        </div>*/}
                            {/*        <p className="text-[10px] text-slate-400 ml-1">*/}
                            {/*            * I tuoi dati bancari verranno usati solo per accreditarti le vendite.*/}
                            {/*        </p>*/}
                            {/*    </div>*/}
                            {/*)}*/}
                        </div>

                        {/* Sezione Consensi */}
                        <div className="space-y-4 mb-6">
                            {/* Privacy Policy - Obbligatoria */}
                            <div className="flex items-start gap-3">
                                <input
                                    type="checkbox"
                                    id="privacy"
                                    required
                                    className="mt-1 h-4 w-4 rounded border-slate-300 text-[#f17829] focus:ring-[#f17829]"
                                />
                                <label htmlFor="privacy" className="text-sm text-slate-600 leading-tight">
                                    Ho letto e accetto la <Link href="/privacy" className="text-[#f17829] hover:underline font-medium">Privacy Policy</Link> e i termini di servizio. *
                                </label>
                            </div>

                            {/* Marketing - Facoltativo */}
                            <div className="flex items-start gap-3">
                                <input
                                    type="checkbox"
                                    id="marketing"
                                    className="mt-1 h-4 w-4 rounded border-slate-300 text-[#f17829] focus:ring-[#f17829]"
                                />
                                <label htmlFor="marketing" className="text-sm text-slate-600 leading-tight">
                                    Acconsento al trattamento dei dati per ricevere offerte promozionali e newsletter dalla community.
                                </label>
                            </div>
                        </div>

                        {/* Google reCAPTCHA Container */}
                        {/*<div className="flex justify-center py-2 bg-slate-50 rounded-xl border border-slate-100">*/}
                        {/*    <ReCAPTCHA*/}
                        {/*        sitekey="TUA_SITE_KEY_QUI" // Sostituisci con la tua chiave fornita da Google*/}
                        {/*        onChange={onChange}*/}
                        {/*        size="normal" // puoi usare "compact" se lo spazio è davvero poco*/}
                        {/*    />*/}
                        {/*</div>*/}

                        <div className="py-2 flex justify-center">
                            {/*Assicurati di aver installato 'react-google-recaptcha' */}
                            {/*<ReCAPTCHA sitekey="TUA_SITE_KEY" onChange={onCaptchaChange} /> */}

                            <div className="flex items-center gap-4 bg-slate-50 border border-slate-200 p-3 rounded-lg w-full">
                                <input
                                    type="checkbox"
                                    id="human-check"
                                    required
                                    className="h-6 w-6 rounded border-slate-300 text-[#f17829] focus:ring-[#f17829]"
                                />
                                <div className="flex flex-col">
                                    <label htmlFor="human-check" className="text-sm font-medium text-slate-700">
                                        Confermo di essere un essere umano
                                    </label>
                                    <span className="text-[10px] text-slate-400 uppercase tracking-widest">Verifica di sicurezza</span>
                                </div>
                                <div className="ml-auto">
                                    <svg viewBox="0 0 24 24" className="h-6 w-6 text-slate-300" fill="currentColor">
                                        <path d="M12 1L3 5v6c0 5.55 3.84 10.74 9 12 5.16-1.26 9-6.45 9-12V5l-9-4zm0 10.99h7c-.47 4.34-3.13 8.23-7 9.47V12H5V6.3l7-3.11v8.8z" />
                                    </svg>
                                </div>
                            </div>
                        </div>
                        {/* BOTTONE INVIO */}
                        <div className="pt-6">
                            <button
                                type="submit"
                                disabled={loading}
                                className={`w-full flex justify-center items-center gap-2 py-4 px-6 border border-transparent rounded-2xl shadow-xl text-lg font-bold text-white transition-all active:scale-[0.98] ${loading ? 'bg-slate-400 cursor-not-allowed' : 'bg-orange-500 hover:bg-orange-600'
                                    }`}
                            >
                                {loading ? (
                                    <span className="flex items-center gap-2">
                                        <svg className="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                        </svg>
                                        Elaborazione...
                                    </span>
                                ) : (
                                    <>
                                        Registrati ora
                                        <ChevronRight size={20} />
                                    </>
                                )}
                            </button>
                        </div>
                    </form>

                    {error && (
                        <p className="mt-4 text-center text-sm font-semibold text-red-600">
                            {error}
                        </p>
                    )}

                    <p className="mt-8 text-center text-sm text-slate-500 font-medium">
                        Hai già un account?{' '}
                        <Link href="/Login" className="text-orange-500 hover:text-orange-600 font-bold underline underline-offset-4">
                            Accedi qui
                        </Link>
                    </p>
                </div>
            </div>
        </div>
    );
};

// Componente Helper per gli Input
//const InputGroup = ({
//    label,
//    name,
//    icon,
//    placeholder,
//    type = "text",
//    required = false,
//    onChange,
//}) => {

//    // SOLO PER I CAMPI TIPO "PHONE"
//    const isPhone = type === "phone";

//    // Funzione di sanitizzazione (ripulisce tutto ciò che non è numero o + in prima posizione)
//    const sanitizePhone = (value) => {
//        let v = value;
//        const plus = v.startsWith("+");
//        v = v.replace(/[^\d]/g, "");   // togli tutto ciò che non è numero
//        if (plus) v = "+" + v;         // rimetti il + solo se era in prima posizione
//        return v;
//    };

//    const handleInput = (e) => {
//        if (isPhone) {
//            const sanitized = sanitizePhone(e.target.value);
//            e.target.value = sanitized;
//        }
//        onChange?.(e);
//    };

//    const handleKeyDown = (e) => {
//        if (!isPhone) return;

//        const allowed = [
//            "Backspace", "Delete", "ArrowLeft", "ArrowRight", "Tab",
//            "Home", "End"
//        ];
//        if (allowed.includes(e.key)) return;

//        // Consenti solo numeri
//        if (/^\d$/.test(e.key)) return;

//        // Consenti + SOLO come primo carattere
//        if (e.key === "+" && e.target.selectionStart === 0 && !e.target.value.includes("+")) {
//            return;
//        }

//        // Altrimenti blocca
//        e.preventDefault();
//    };

//    const handleBeforeInput = (e) => {
//        if (!isPhone) return;
//        const inputType = e.nativeEvent?.inputType;
//        if (inputType && inputType.startsWith("delete")) return;

//        const char = e.data;
//        if (!char) return;

//        const target = e.target;
//        const nextValue =
//            target.value.slice(0, target.selectionStart) +
//            char +
//            target.value.slice(target.selectionEnd);

//        // Permetti solo + come primo carattere e cifre
//        if (!/^\+?\d*$/.test(nextValue)) {
//            e.preventDefault();
//        }
//    };

//    return (
//        <div className="flex flex-col gap-2">
//            <label className="text-sm font-bold text-slate-700 ml-1">
//                {label}
//                {required && <span className="text-orange-500 ml-1">*</span>}
//            </label>

//            <div className="relative flex items-center group">
//                {icon && (
//                    <div className="absolute left-4 text-slate-400 group-focus-within:text-orange-500 transition-colors">
//                        {icon}
//                    </div>
//                )}

//                <input
//                    type="tel"
//                    name={name}
//                    onChange={handleInput}
//                    onBeforeInput={handleBeforeInput}
//                    onKeyDown={handleKeyDown}
//                    placeholder={placeholder}
//                    required={required}
//                    inputMode={isPhone ? "numeric" : undefined}
//                    className={`w-full ${icon ? 'pl-11' : 'pl-4'} pr-4 py-3.5 bg-slate-50 border border-slate-200 rounded-2xl outline-none focus:border-orange-400 focus:ring-4 focus:ring-orange-400/10 transition-all text-slate-600 font-medium placeholder:text-slate-300`}
//                />
//            </div>
//        </div>
//    );
//};


// Componente Helper per la scelta Spedizione/Scambio
//const SelectionCard = ({ active, onClick, icon, title, desc }) => (
//    <div
//        onClick={onClick}
//        className={`cursor-pointer p-5 rounded-2xl border-2 transition-all flex flex-col gap-2 ${active ? 'border-orange-500 bg-orange-50/50 ring-4 ring-orange-500/10' : 'border-slate-100 bg-white hover:border-slate-200 shadow-sm'
//            }`}
//    >
//        <div className={active ? 'text-orange-500' : 'text-slate-400'}>{icon}</div>
//        <div>
//            <h5 className={`text-sm font-black ${active ? 'text-orange-600' : 'text-slate-700'}`}>{title}</h5>
//            <p className="text-[11px] font-medium text-slate-500 leading-tight">{desc}</p>
//        </div>
//    </div>
//);

export default Register;