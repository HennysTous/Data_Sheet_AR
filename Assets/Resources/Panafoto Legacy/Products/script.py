import os
import pandas as pd

def buscar_carpetas_hisense(directorio):
    # Lista para almacenar los nombres de las carpetas encontradas
    carpetas_hisense = []
    
    # Recorrer el directorio y sus subdirectorios
    for root, dirs, files in os.walk(directorio):
        for dir_name in dirs:
            if "Hisense" in dir_name:
                # Agregar la ruta completa de la carpeta a la lista
                carpetas_hisense.append(dir_name)
    
    return carpetas_hisense

def exportar_a_excel(carpetas, archivo_salida):
    # Crear un DataFrame de pandas a partir de la lista de carpetas
    df = pd.DataFrame(carpetas, columns=["Carpetas Hisense"])
    
    # Exportar el DataFrame a un archivo Excel
    df.to_excel(archivo_salida, index=False)

if __name__ == "__main__":
    # Obtener el directorio actual donde se encuentra el script
    directorio_a_buscar = os.path.dirname(os.path.abspath(__file__))
    archivo_excel = "carpetas_hisense.xlsx"
    
    # Buscar carpetas y exportar a Excel
    carpetas_encontradas = buscar_carpetas_hisense(directorio_a_buscar)
    exportar_a_excel(carpetas_encontradas, archivo_excel)
    
    print(f"Se han encontrado {len(carpetas_encontradas)} carpetas con 'Hisense' en su nombre.")