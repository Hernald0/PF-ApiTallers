using System;
using System.Collections.Generic;
using System.Text;

namespace UTNApiTalleres.Data.Repositorio.Interfaz

{
    interface ICrudObjeto<objeto>
    {
        //Crear el nuevo objeto en la base
        void create(objeto obj);

        //actualizar el objeto en la base
        void update(objeto obj);

        //eliminar el objeto de la base 
        int delete(int id);

        //recuperar un objeto desde la base
        bool find(objeto obj);

        objeto find(int id);

        //recuperar todos los objetos desde la base
        List<objeto> findAll();


    }
}
