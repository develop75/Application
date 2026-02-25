'use client';

import { useState, useEffect, useRef } from 'react';

import Link from 'next/link';
import { User, Mail, Lock, Phone, MapPin, Hash, Truck, Handshake, ChevronRight } from 'lucide-react';
import ReCAPTCHA from "react-google-recaptcha";

import ProvinceSelect from '@/components/ProvinceSelect'
import SearchableSelect from '@/components/SearchableSelect'


import { getProvinces } from '../services/dictionary'



const Register = () => {

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
        }
    };


    const onChange = (value) => {
        console.log("Captcha value:", value);
        // Qui puoi salvare il valore nello stato per inviarlo al backend
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
                    <form className="space-y-6">

                        {/* SEZIONE 1: ANAGRAFICA */}
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <InputGroup label="Nome" name="nome" icon={<User size={18} />} placeholder="Mario" onChange={handleChange} />
                            <InputGroup label="Cognome" name="cognome" icon={<User size={18} />} placeholder="Rossi" onChange={handleChange} />
                            <InputGroup label="Codice Fiscale" name="cf" icon={<Hash size={18} />} placeholder="RSSMRA..." onChange={handleChange} />
                            <InputGroup label="Cellulare" name="cellulare" icon={<Phone size={18} />} placeholder="+39 333..." onChange={handleChange} />
                        </div>

                        <hr className="border-slate-50" />

                        {/* SEZIONE 2: RESIDENZA */}
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <div className="md:col-span-2">
                                <InputGroup label="Indirizzo" name="indirizzo" icon={<MapPin size={18} />} placeholder="Via Roma, 10" onChange={handleChange} />
                            </div>
                            {/*<InputGroup label="Città" name="citta" icon={<MapPin size={18} />} placeholder="Milano" onChange={handleChange} />*/}
               
                            {/* SELECT PROVINCE */}
                            <SearchableSelect
                                label="Provincia"
                                name="provinceId"
                                items={provinces}
                                value={formData.provinceId}
                                itemLabel="provinceName"
                                itemBadge="provinceCode"
                                onChange={handleChange}
                                icon={<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" /><path d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" /></svg>}
                            />

                            {/* SELECT CITTÀ */}
                            <SearchableSelect
                                label="Città"
                                name="cityId"
                                items={availableCities}
                                value={formData.cityId}
                                itemLabel="cityName"
                                itemBadge="cityCode"
                                onChange={handleChange}
                                placeholder={formData.provinceId ? "Seleziona città..." : "Scegli prima una provincia"}
                                icon={<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" /></svg>}
                            />
                            <div className="grid grid-cols-2 gap-4">
                                <InputGroup label="CAP" name="cap" placeholder="20100" onChange={handleChange} />
                                <InputGroup label="Prov." name="provincia" placeholder="MI" onChange={handleChange} />
                            </div>
                        </div>

                        <hr className="border-slate-50" />

                        {/* SEZIONE 3: ACCOUNT */}
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <InputGroup label="Mail" name="mail" icon={<Mail size={18} />} placeholder="mario88" onChange={handleChange} />
                            <InputGroup label="Password" name="password" type="password" icon={<Lock size={18} />} placeholder="••••••••" onChange={handleChange} />
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
                            {canShip && (
                                <div className="space-y-2 animate-in fade-in slide-in-from-top-2 duration-300">
                                    <label className="text-xs font-bold text-slate-600 uppercase ml-1">
                                        Il tuo IBAN (per ricevere i pagamenti)
                                    </label>
                                    <div className="relative">
                                        <input
                                            type="text"
                                            placeholder="IT 00 X 00000 00000 000000000000"
                                            className="w-full p-3 bg-white border border-[#f17829] rounded-xl focus:outline-none focus:ring-2 focus:ring-[#f17829]/20 font-mono text-sm uppercase"
                                            required={canShip}
                                        />
                                        <div className="absolute right-3 top-3 text-[#f17829]">
                                            <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
                                            </svg>
                                        </div>
                                    </div>
                                    <p className="text-[10px] text-slate-400 ml-1">
                                        * I tuoi dati bancari verranno usati solo per accreditarti le vendite.
                                    </p>
                                </div>
                            )}
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
                            <button type="submit" className="w-full flex justify-center items-center gap-2 py-4 px-6 border border-transparent rounded-2xl shadow-xl text-lg font-bold text-white bg-orange-500 hover:bg-orange-600 focus:outline-none focus:ring-4 focus:ring-orange-500/20 transition-all active:scale-[0.98]">
                                Registrati ora
                                <ChevronRight size={20} />
                            </button>
                        </div>
                    </form>

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
const InputGroup = ({ label, name, icon, placeholder, type = "text", onChange }) => (
    <div className="flex flex-col gap-2">
        <label className="text-sm font-bold text-slate-700 ml-1">{label}</label>
        <div className="relative flex items-center group">
            {icon && <div className="absolute left-4 text-slate-400 group-focus-within:text-orange-500 transition-colors">{icon}</div>}
            <input
                type={type}
                name={name}
                onChange={onChange}
                placeholder={placeholder}
                className={`w-full ${icon ? 'pl-11' : 'pl-4'} pr-4 py-3.5 bg-slate-50 border border-slate-200 rounded-2xl outline-none focus:border-orange-400 focus:ring-4 focus:ring-orange-400/10 transition-all text-slate-600 font-medium placeholder:text-slate-300`}
            />
        </div>
    </div>
);

// Componente Helper per la scelta Spedizione/Scambio
const SelectionCard = ({ active, onClick, icon, title, desc }) => (
    <div
        onClick={onClick}
        className={`cursor-pointer p-5 rounded-2xl border-2 transition-all flex flex-col gap-2 ${active ? 'border-orange-500 bg-orange-50/50 ring-4 ring-orange-500/10' : 'border-slate-100 bg-white hover:border-slate-200 shadow-sm'
            }`}
    >
        <div className={active ? 'text-orange-500' : 'text-slate-400'}>{icon}</div>
        <div>
            <h5 className={`text-sm font-black ${active ? 'text-orange-600' : 'text-slate-700'}`}>{title}</h5>
            <p className="text-[11px] font-medium text-slate-500 leading-tight">{desc}</p>
        </div>
    </div>
);

export default Register;