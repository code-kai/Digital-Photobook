namespace Digitales_Fotobuch.code
{
    public class Tag
    {
        private string name           = "";

        private bool   active         = false;
        private bool   changePossible = false;

        public Tag(string name, bool active, bool changePossible)
        {
            this.name           = name;
            this.active         = active;
            this.changePossible = changePossible;
        }

        public string GetName()
        {
            return name;
        }

        public bool SetName(string newName)
        {
            //Darf der Tag geandert werden?
            if (changePossible == true)
            {
                //Ein Tag kann nur einmal geandert werden (wenn er neu ist)
                changePossible = false;
                name           = newName;

                //Melden das Name geaendert wurde
                return true;
            }
            else
            {
                //Melden das Name nicht geaendert wurde
                return false;
            }
        }

        public bool IsChangePossible()
        {
            return changePossible;
        }

        public bool IsActive()
        {
            return active;
        }

        public void SetActive()
        {
            active = true;
        }
        public void SetInactive()
        {
            active = false;
        }
    }
}
