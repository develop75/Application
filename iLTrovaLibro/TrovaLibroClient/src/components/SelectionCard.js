
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

export default SelectionCard;