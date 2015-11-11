using System;
using System.Diagnostics;
using System.Collections;


namespace NS_GMath
{
    public class Composite : IEnumerable
    {    
        /*
         *        MEMBERS
         */
        ArrayList components;

        /*
         *        CONSTRUCTORS
         */
        public Composite()
        {
            this.components=new ArrayList();
        }

        /*
         *        PROPERTIES
         */

        public int NumComponent
        {
            get { return this.components.Count; }
        }

        /*
         *        METHODS
         */
        public Component ComponentByPoz(int pozComponent)
        {
            if ((pozComponent<0)||(pozComponent>=this.NumComponent))
                return null;
            return this.components[pozComponent] as Component;

        }

        public Component ComponentByIndGlyph(int indGlyph)
        {
            foreach (Component component in this.components)
            {
                if (component.IndexGlyphComponent==indGlyph)
                {
                    return component;
                }
            }
            return null;
        }
    
        public int IndGlyphByPozCont(int pozCont)
        {
            /*
             *        returns IND_UNDEFINED on failure
             */
            int numCont=0;
            foreach (Component component in this.components)
            {
                if (component.NumCont==GConsts.IND_UNINITIALIZED)
                    return GConsts.IND_UNDEFINED;
                numCont+=component.NumKnot;
                if (pozCont<numCont)
                    return component.IndexGlyphComponent;
            }
            return GConsts.IND_UNDEFINED;
        }

        public int IndGlyphByIndKnot(int indKnot)
        {
            /*
             *        returns IND_UNDEFINED on failure
             */
            int numKnot=0;
            foreach (Component component in this.components)
            {
                if (component.NumKnot==GConsts.IND_UNINITIALIZED)
                    return GConsts.IND_UNDEFINED;
                numKnot+=component.NumKnot;
                if (indKnot<numKnot)
                    return component.IndexGlyphComponent;
            }
            return GConsts.IND_UNDEFINED;
        }
        
        
        public void ClearDestroy()
        {
            foreach (Component component in this.components)
            {
                component.ClearRelease();
            }
            this.components.Clear();
            this.components=null;
        }
        public void ClearReset()
        {
            foreach (Component component in this.components)
            {
                component.ClearReset();
            }
            this.components.Clear();
        }
        public void ClearRelease()
        {
            this.ClearReset();
            this.components=null;
        }
        public IEnumerator GetEnumerator() // IEnumerable
        {
            return this.components.GetEnumerator();
        }

        public void AddComponent(Component component)
        {
            if (component==null)
                return;
            this.components.Add(component);
        }

/*
        internal void LoadContent()
        {

        }

        internal void UnloadContent()
        {

        }
*/
    };
}