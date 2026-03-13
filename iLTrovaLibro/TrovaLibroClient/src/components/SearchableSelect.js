"use client";
import { useState, useEffect, useRef, forwardRef, useImperativeHandle } from 'react';

const SearchableSelect = forwardRef(function SearchableSelect(
    {
        label,
        name,
        value,
        items = [],
        itemKey = "id",
        itemLabel = "name",
        itemBadge = null,
        onChange,
        required = false,
        placeholder = "Seleziona...",
        icon = null
    },
    ref
) {
    const [searchTerm, setSearchTerm] = useState("");
    const [isOpen, setIsOpen] = useState(false);
    const [isInvalid, setIsInvalid] = useState(false);
    const wrapperRef = useRef(null);
    const hiddenInputRef = useRef(null);

    const selectedItem = items.find(item => item?.[itemKey] === value);
    const displayLabel = selectedItem ? selectedItem[itemLabel] : "";

    const filteredItems = items.filter(item =>
        item?.[itemLabel]?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        (itemBadge && item?.[itemBadge]?.toLowerCase().includes(searchTerm.toLowerCase()))
    );

    useEffect(() => {
        const handleClickOutside = (e) => {
            if (wrapperRef.current && !wrapperRef.current.contains(e.target)) setIsOpen(false);
        };
        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, []);

    // Reset UI errore se cambia dall’esterno
    useEffect(() => {
        if (value) setIsInvalid(false);
    }, [value]);

    // API imperativa: validate() e focus()
    useImperativeHandle(ref, () => ({
        validate: () => {
            const ok = !(required && (value === "" || value === null || value === undefined));
            setIsInvalid(!ok);
            if (!ok) {
                // porta focus al contenitore (o all’input invisibile)
                hiddenInputRef.current?.focus();
            }
            return ok;
        },
        focus: () => {
            hiddenInputRef.current?.focus();
            setIsOpen(true);
        },
        setInvalid: (flag) => setIsInvalid(!!flag),
    }));

    const handleSelect = (item) => {
        setIsOpen(false);
        setIsInvalid(false);
        setSearchTerm("");
        onChange?.({
            target: {
                name: name,
                value: item[itemKey],
                originalItem: item
            }
        });
    };

    return (
        <div className="relative w-full" ref={wrapperRef}>
            {label && (
                <label className={`block text-sm font-bold mb-2 ml-1 uppercase transition-colors ${isInvalid ? 'text-red-600' : 'text-slate-800'}`}>
                    {label} {required && <span className="text-orange-500">*</span>}
                </label>
            )}

            {/* INPUT INVISIBILE ma parte del form */}

            <input
                ref={hiddenInputRef}
                type="text"
                name={name}
                autoComplete="off"
                value={value ?? ""}
                required={required}
                readOnly
                onInvalid={(e) => {
                    e.preventDefault();
                    setIsInvalid(true);
                }}
                className="absolute inset-0 w-full h-full opacity-0 pointer-events-none z-0"
                tabIndex={0}
                style={{ background: "transparent" }}
            />


            <div
                onClick={() => {
                    // aiutiamo il focus (utile su Safari/iOS)
                    hiddenInputRef.current?.focus();
                    setIsOpen(!isOpen);
                }}
                className={`w-full flex items-center justify-between p-4 bg-[#f8fafc] border-2 rounded-2xl cursor-pointer transition-all ${isInvalid
                        ? 'border-red-500 bg-red-50 shadow-[0_0_0_4px_rgba(239,68,68,0.1)]'
                        : isOpen ? 'border-orange-500 ring-4 ring-orange-500/10' : 'border-slate-200'
                    } ${items.length === 0 ? 'opacity-60 cursor-not-allowed' : ''}`}
            >
                <div className="flex items-center gap-3 overflow-hidden">
                    {icon && <div className={`${isInvalid ? 'text-red-500' : 'text-slate-400'} shrink-0`}>{icon}</div>}
                    <span className={`truncate ${displayLabel ? "text-slate-700 font-bold" : "text-slate-400 font-medium"}`}>
                        {displayLabel || (items.length === 0 ? "Caricamento..." : placeholder)}
                    </span>
                </div>
                <svg className={`h-5 w-5 ${isInvalid ? 'text-red-500' : 'text-slate-400'} transition-transform ${isOpen ? 'rotate-180' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7" />
                </svg>
            </div>

            {isInvalid && (
                <span className="absolute -bottom-5 left-1 text-[10px] font-black text-red-600 uppercase tracking-tighter">
                    Seleziona un'opzione dall'elenco
                </span>
            )}

            {isOpen && items.length > 0 && (
                <div className="absolute z-[100] w-full mt-2 bg-white border border-slate-200 rounded-2xl shadow-2xl overflow-hidden animate-in fade-in zoom-in duration-200">
                    <div className="p-3 bg-slate-50 border-b border-slate-100">
                        <input
                            type="text"
                            autoFocus
                            placeholder="Cerca..."
                            className="w-full p-2.5 text-sm bg-white border border-slate-200 rounded-xl outline-none focus:border-orange-500"
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </div>
                    <ul className="max-h-64 overflow-y-auto py-1">
                        {filteredItems.length > 0 ? (
                            filteredItems.map((item) => (
                                <li
                                    key={item[itemKey]}
                                    onClick={() => handleSelect(item)}
                                    className="px-5 py-3 text-sm text-slate-600 hover:bg-orange-50 hover:text-orange-600 cursor-pointer flex justify-between items-center transition-colors"
                                >
                                    <span className="font-semibold">{item[itemLabel]}</span>
                                    {itemBadge && item[itemBadge] && (
                                        <span className="text-[10px] font-black bg-slate-100 px-2 py-0.5 rounded text-slate-400 uppercase">
                                            {item[itemBadge]}
                                        </span>
                                    )}
                                </li>
                            ))
                        ) : (
                            <li className="px-5 py-3 text-sm text-slate-400 text-center">Nessun risultato</li>
                        )}
                    </ul>
                </div>
            )}
        </div>
    );
});

export default SearchableSelect;