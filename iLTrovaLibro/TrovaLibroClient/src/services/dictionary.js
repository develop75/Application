


/**
 * Cerca tutte le provincie
 * @returns
 */
export const getProvinces = async () => {
    try {
        const response = await fetch('https:/localhost:7065/api/ProvinceDetail', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'X-App-Identify-Token': 'IlTuoCodiceSegretoPrivato'
            }
        });
        const data = await response.json();

        return data;

    } catch (error) {
        console.error("Errore nel caricamento province:", error);

        throw new Error(error);
    }
};