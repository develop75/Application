"use client";

import { useState, useRef } from "react";

const InputGroup = ({
    label,
    name,
    icon,
    placeholder,
    type = "text",
    required = false,
    onChange,
}) => {
    const [isInvalid, setIsInvalid] = useState(false);
    const inputRef = useRef(null);

    // Detect if is phone
    const isPhone = type === "phone";

    // Sanitize phone input
    const sanitizePhone = (value) => {
        const hasPlus = value.startsWith("+");
        let v = value.replace(/[^\d]/g, "");
        if (hasPlus) v = "+" + v;
        return v;
    };

    const handleChange = (e) => {
        if (isPhone) {
            e.target.value = sanitizePhone(e.target.value);
        }
        setIsInvalid(false);
        onChange?.(e);
    };

    const handleKeyDown = (e) => {
        if (!isPhone) return;

        const allowed = [
            "Backspace",
            "Delete",
            "ArrowLeft",
            "ArrowRight",
            "Tab",
            "Home",
            "End",
        ];
        if (allowed.includes(e.key)) return;

        if (/^\d$/.test(e.key)) return;

        if (e.key === "+" && e.target.selectionStart === 0 && !e.target.value.includes("+"))
            return;

        e.preventDefault();
    };

    return (
        <div className="flex flex-col gap-2 relative">
            {/* LABEL */}
            <label className={`text-sm font-bold ml-1 uppercase transition-colors ${isInvalid ? "text-red-600" : "text-slate-700"
                }`}>
                {label}
                {required && <span className="text-orange-500 ml-1">*</span>}
            </label>

            {/* INPUT WRAPPER CON STILE UGUALE AL SEARCHABLESELECT */}
            <div
                className={`relative flex items-center group transition-all border-2 rounded-2xl p-3.5 bg-[#f8fafc]
                    ${isInvalid
                        ? "border-red-500 bg-red-50 shadow-[0_0_0_4px_rgba(239,68,68,0.1)]"
                        : "border-slate-200 focus-within:border-orange-500 focus-within:ring-4 focus-within:ring-orange-500/10"
                    }
                `}
            >
                {icon && (
                    <div className={`absolute left-4 transition-colors ${isInvalid ? "text-red-500" : "text-slate-400 group-focus-within:text-orange-500"
                        }`}>
                        {icon}
                    </div>
                )}

                <input
                    ref={inputRef}
                    type={isPhone ? "tel" : type}
                    name={name}
                    placeholder={placeholder}
                    required={required}
                    inputMode={isPhone ? "numeric" : undefined}
                    onChange={handleChange}
                    onKeyDown={handleKeyDown}
                    onInvalid={(e) => {
                        e.preventDefault();
                        setIsInvalid(true);
                    }}
                    className={`w-full bg-transparent outline-none font-medium text-slate-700
                        ${icon ? "pl-11" : "pl-1"} pr-4
                        ${isInvalid ? "text-red-600" : ""}
                    `}
                />
            </div>

            {/* MESSAGGIO DI ERRORE */}
            {isInvalid && (
                <span className="absolute -bottom-4 left-1 text-[10px] font-black text-red-600 uppercase tracking-tighter">
                    Questo campo è obbligatorio
                </span>
            )}
        </div>
    );
};

export default InputGroup;