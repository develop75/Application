"use client";
import { useState, useEffect, useRef } from 'react';

export default function SearchableSelect({
    label,
    name,
    value,
    items = [], // Lista di oggetti (es. province o città)
    itemKey = "id", // La chiave da usare come valore (es. "id")
    itemLabel = "name", // La chiave da mostrare (es. "provinceName" o "cityName")
    itemBadge = null, // Chiave opzionale per un badge (es. "provinceCode")
    onChange,
    placeholder = "Seleziona...",
    icon = null // Puoi passare un'icona SVG come prop
}) {
    const [searchTerm, setSearchTerm] = useState("");
    const [isOpen, setIsOpen] = useState(false);
    const wrapperRef = useRef(null);

    // Trova l'oggetto selezionato per visualizzarlo nel trigger
    const selectedItem = items.find(item => item[itemKey] === value);
    const displayLabel = selectedItem ? selectedItem[itemLabel] : "";

    const filteredItems = items.filter(item =>
        item[itemLabel]?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        (itemBadge && item[itemBadge]?.toLowerCase().includes(searchTerm.toLowerCase()))
    );

    useEffect(() => {
        const handleClickOutside = (e) => {
            if (wrapperRef.current && !wrapperRef.current.contains(e.target)) setIsOpen(false);
        };
        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, []);

    const handleSelect = (item) => {
        setIsOpen(false);
        setSearchTerm("");
        onChange({
            target: {
                name: name,
                value: item[itemKey],
                originalItem: item // Passiamo l'oggetto intero per logiche extra
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
                    } ${items.length === 0 ? 'opacity-60 cursor-not-allowed' : ''}`}
            >
                <div className="flex items-center gap-3 overflow-hidden">
                    {icon && <div className="text-slate-400 shrink-0">{icon}</div>}
                    <span className={`truncate ${displayLabel ? "text-slate-700 font-medium" : "text-slate-400"}`}>
                        {displayLabel || (items.length === 0 ? "Caricamento..." : placeholder)}
                    </span>
                </div>
                <svg className={`h-5 w-5 text-slate-400 transition-transform ${isOpen ? 'rotate-180' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7" />
                </svg>
            </div>

            {isOpen && items.length > 0 && (
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
                        {filteredItems.map((item) => (
                            <li
                                key={item[itemKey]}
                                onClick={() => handleSelect(item)}
                                className="px-5 py-3 text-sm text-slate-600 hover:bg-[#f17829]/5 hover:text-[#f17829] cursor-pointer flex justify-between items-center transition-colors"
                            >
                                <span className="font-medium">{item[itemLabel]}</span>
                                {itemBadge && item[itemBadge] && (
                                    <span className="text-[10px] font-black bg-slate-100 px-2 py-0.5 rounded text-slate-400 uppercase">
                                        {item[itemBadge]}
                                    </span>
                                )}
                            </li>
                        ))}
                    </ul>
                </div>
            )}
        </div>
    );
}