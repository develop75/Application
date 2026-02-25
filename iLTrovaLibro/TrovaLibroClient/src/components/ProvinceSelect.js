"use client";
import { useState, useEffect, useRef } from 'react';

export default function ProvinceSelect({
    label,
    name,
    value, // L'ID o il codice della provincia salvato nello stato del form
    onChange,
    placeholder = "Seleziona..."
}) {
    const [provinces, setProvinces] = useState([]);
    const [filteredProvinces, setFilteredProvinces] = useState([]);
    const [searchTerm, setSearchTerm] = useState("");
    const [isOpen, setIsOpen] = useState(false);
    const wrapperRef = useRef(null);

    // Trova il nome da visualizzare in base al value attuale
    const currentProvince = provinces.find(p => p.id === value || p.provinceCode === value);
    const displayValue = currentProvince ? `${currentProvince.provinceName} (${currentProvince.provinceCode})` : "";

    useEffect(() => {
        const fetchProvinces = async () => {
            try {
                const response = await fetch('https://localhost:7065/api/ProvinceDetail', {
                    headers: { 'X-App-Identify-Token': 'IlTuoCodiceSegretoPrivato' }
                });
                const data = await response.json();
                setProvinces(data);
                setFilteredProvinces(data);
            } catch (error) { console.error("Errore province:", error); }
        };
        fetchProvinces();
    }, []);

    useEffect(() => {
        const results = provinces.filter(p =>
            p.provinceName.toLowerCase().includes(searchTerm.toLowerCase()) ||
            p.provinceCode.toLowerCase().includes(searchTerm.toLowerCase())
        );
        setFilteredProvinces(results);
    }, [searchTerm, provinces]);

    // Gestione clic fuori per chiudere
    useEffect(() => {
        const handleClickOutside = (e) => {
            if (wrapperRef.current && !wrapperRef.current.contains(e.target)) setIsOpen(false);
        };
        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, []);

    const handleSelect = (province) => {
        setIsOpen(false);
        setSearchTerm("");

        // Simula un evento target per essere compatibile con handleChange(e)
        onChange({
            target: {
                name: name,
                value: province.id, // o province.provinceCode in base a cosa salva il tuo DB
                // Passiamo anche l'intero oggetto per comodità (es. per caricare le città)
                provinceData: province
            }
        });
    };

    return (
        <div className="relative w-full" ref={wrapperRef}>
            {label && (
                <label className="block text-sm font-bold text-slate-800 mb-2 ml-1 uppercase">
                    {label}
                </label>
            )}

            <div
                onClick={() => setIsOpen(!isOpen)}
                className={`w-full flex items-center justify-between p-4 bg-[#f8fafc] border rounded-2xl cursor-pointer transition-all ${isOpen ? 'border-[#f17829] ring-2 ring-[#f17829]/10' : 'border-slate-200'
                    }`}
            >
                <div className="flex items-center gap-3 overflow-hidden">
                    <svg className="w-5 h-5 text-slate-400 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                    </svg>
                    <span className={`truncate ${displayValue ? "text-slate-700 font-medium" : "text-slate-400"}`}>
                        {displayValue || placeholder}
                    </span>
                </div>
                <svg className={`h-5 w-5 text-slate-400 transition-transform duration-200 ${isOpen ? 'rotate-180' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7" />
                </svg>
            </div>

            {isOpen && (
                <div className="absolute z-[100] w-full mt-2 bg-white border border-slate-200 rounded-2xl shadow-2xl overflow-hidden animate-in fade-in slide-in-from-top-2">
                    <div className="p-3 bg-slate-50 border-b border-slate-100">
                        <input
                            type="text"
                            autoFocus
                            placeholder="Cerca..."
                            className="w-full p-2.5 text-sm bg-white border border-slate-200 rounded-xl outline-none focus:border-[#f17829]"
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </div>
                    <ul className="max-h-64 overflow-y-auto py-1">
                        {filteredProvinces.map((p) => (
                            <li
                                key={p.id}
                                onClick={() => handleSelect(p)}
                                className="px-5 py-3 text-sm text-slate-600 hover:bg-[#f17829]/5 hover:text-[#f17829] cursor-pointer flex justify-between items-center transition-colors"
                            >
                                <span className="font-medium">{p.provinceName}</span>
                                <span className="text-[10px] font-black bg-slate-100 px-2 py-0.5 rounded text-slate-400 uppercase tracking-tighter">
                                    {p.provinceCode}
                                </span>
                            </li>
                        ))}
                    </ul>
                </div>
            )}
        </div>
    );
}