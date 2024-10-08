from typing import Any
import os
import json


class Product:

    __name = ""
    __description = ""

    def __init__(self, name) -> None:

        self.__name = f"Interacción con Retail Xperience: {name}"
        self.__description = f"Felicidades, has obtenido un pop por interactuar con un el producto Retail Xperience {name} de iLikeit."

    def to_dict(self) -> dict:

        return {"name": self.__name, "description": self.__description}


class main:

    path = "."

    @staticmethod
    def scan_folders(path: str) -> list:

        valid_folders = [ ]

        try:
            for root, dirs, _ in os.walk(path):
                for dir in dirs:
                    if "NO ENCONTRADO" not in dir:
                        valid_folders.append(os.path.join(root, dir))
        except Exception as e:

            print(f"Ocurrio un error: {e}")

        return valid_folders

    @staticmethod
    def create_products(folders: list) -> list:

        products = []
        for folder in folders:

            products.append(Product(os.path.basename(folder)))

        return products

    @staticmethod
    def create_json(data: list, output_file: set) -> None:

        try:
            with open(output_file, "w", encoding="utf-8") as file:
                json.dump(data, file, ensure_ascii = False,indent=4)
                print(f"Archivo JSON generado correctamente en {output_file}")
        except Exception as e:
            print(f"Ocurrió un error al generar el archivo JSON: {e}")

    def main(self) -> None:
        valid_folders = self.scan_folders(self.path)

        if valid_folders:
            print("Carpetas válidas encontradas:")
            for folder in valid_folders:
                print(folder)

            folder_names = [os.path.basename(folder) for folder in valid_folders]
            self.create_json(folder_names, "carpetas.json")

            products = self.create_products(valid_folders)
            products_data = [product.to_dict() for product in products]
            self.create_json(products_data, "products.json")

            for product in products:
                print(f"Producto creado: {product._Product__name}")
        else:
            print("No se encontraron carpetas válidas.")


if __name__ == "__main__":
    main = main()
    main.main()
