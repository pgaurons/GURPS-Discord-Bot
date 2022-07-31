
using System.Xml.Serialization;

namespace Gao.Gurps.Model
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class advantage_list
    {

        private advantage_listAdvantage_container[] advantage_containerField;

        private string unique_idField;

        private byte versionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("advantage_container")]
        public advantage_listAdvantage_container[] advantage_container
        {
            get
            {
                return this.advantage_containerField;
            }
            set
            {
                this.advantage_containerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string unique_id
        {
            get
            {
                return this.unique_idField;
            }
            set
            {
                this.unique_idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_container
    {

        private object[] itemsField;

        private ItemsChoiceType3[] itemsElementNameField;

        private byte versionField;

        private string openField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("advantage", typeof(advantage_listAdvantage_containerAdvantage))]
        [System.Xml.Serialization.XmlElementAttribute("advantage_container", typeof(advantage_listAdvantage_containerAdvantage_container))]
        [System.Xml.Serialization.XmlElementAttribute("name", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("notes", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("reference", typeof(string))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType3[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string open
        {
            get
            {
                return this.openField;
            }
            set
            {
                this.openField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantage
    {

        private object[] itemsField;

        private ItemsChoiceType2[] itemsElementNameField;

        private byte versionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("attribute_bonus", typeof(advantage_listAdvantage_containerAdvantageAttribute_bonus))]
        [System.Xml.Serialization.XmlElementAttribute("base_points", typeof(sbyte))]
        [System.Xml.Serialization.XmlElementAttribute("categories", typeof(advantage_listAdvantage_containerAdvantageCategories))]
        [System.Xml.Serialization.XmlElementAttribute("cr", typeof(advantage_listAdvantage_containerAdvantageCR))]
        [System.Xml.Serialization.XmlElementAttribute("dr_bonus", typeof(advantage_listAdvantage_containerAdvantageDr_bonus))]
        [System.Xml.Serialization.XmlElementAttribute("levels", typeof(byte))]
        [System.Xml.Serialization.XmlElementAttribute("melee_weapon", typeof(advantage_listAdvantage_containerAdvantageMelee_weapon))]
        [System.Xml.Serialization.XmlElementAttribute("modifier", typeof(advantage_listAdvantage_containerAdvantageModifier))]
        [System.Xml.Serialization.XmlElementAttribute("name", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("notes", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("points_per_level", typeof(sbyte))]
        [System.Xml.Serialization.XmlElementAttribute("prereq_list", typeof(advantage_listAdvantage_containerAdvantagePrereq_list))]
        [System.Xml.Serialization.XmlElementAttribute("reference", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("skill_bonus", typeof(advantage_listAdvantage_containerAdvantageSkill_bonus))]
        [System.Xml.Serialization.XmlElementAttribute("type", typeof(string))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType2[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageAttribute_bonus
    {

        private advantage_listAdvantage_containerAdvantageAttribute_bonusAttribute attributeField;

        private advantage_listAdvantage_containerAdvantageAttribute_bonusAmount amountField;

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantageAttribute_bonusAttribute attribute
        {
            get
            {
                return this.attributeField;
            }
            set
            {
                this.attributeField = value;
            }
        }

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantageAttribute_bonusAmount amount
        {
            get
            {
                return this.amountField;
            }
            set
            {
                this.amountField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageAttribute_bonusAttribute
    {

        private string limitationField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string limitation
        {
            get
            {
                return this.limitationField;
            }
            set
            {
                this.limitationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageAttribute_bonusAmount
    {

        private string per_levelField;

        private sbyte valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string per_level
        {
            get
            {
                return this.per_levelField;
            }
            set
            {
                this.per_levelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public sbyte Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageCategories
    {

        private string[] categoryField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("category")]
        public string[] category
        {
            get
            {
                return this.categoryField;
            }
            set
            {
                this.categoryField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageCR
    {

        private string adjField;

        private byte valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string adj
        {
            get
            {
                return this.adjField;
            }
            set
            {
                this.adjField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public byte Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageDr_bonus
    {

        private string locationField;

        private advantage_listAdvantage_containerAdvantageDr_bonusAmount amountField;

        /// <remarks/>
        public string location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantageDr_bonusAmount amount
        {
            get
            {
                return this.amountField;
            }
            set
            {
                this.amountField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageDr_bonusAmount
    {

        private string per_levelField;

        private byte valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string per_level
        {
            get
            {
                return this.per_levelField;
            }
            set
            {
                this.per_levelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public byte Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageMelee_weapon
    {

        private string damageField;

        private string usageField;

        private string reachField;

        private string parryField;

        private string blockField;

        private advantage_listAdvantage_containerAdvantageMelee_weaponDefault[] defaultField;

        /// <remarks/>
        public string damage
        {
            get
            {
                return this.damageField;
            }
            set
            {
                this.damageField = value;
            }
        }

        /// <remarks/>
        public string usage
        {
            get
            {
                return this.usageField;
            }
            set
            {
                this.usageField = value;
            }
        }

        /// <remarks/>
        public string reach
        {
            get
            {
                return this.reachField;
            }
            set
            {
                this.reachField = value;
            }
        }

        /// <remarks/>
        public string parry
        {
            get
            {
                return this.parryField;
            }
            set
            {
                this.parryField = value;
            }
        }

        /// <remarks/>
        public string block
        {
            get
            {
                return this.blockField;
            }
            set
            {
                this.blockField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("default")]
        public advantage_listAdvantage_containerAdvantageMelee_weaponDefault[] @default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageMelee_weaponDefault
    {

        private string typeField;

        private string nameField;

        private byte modifierField;

        /// <remarks/>
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public byte modifier
        {
            get
            {
                return this.modifierField;
            }
            set
            {
                this.modifierField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageModifier
    {

        private string nameField;

        private advantage_listAdvantage_containerAdvantageModifierCost costField;

        private byte levelsField;

        private bool levelsFieldSpecified;

        private string affectsField;

        private string referenceField;

        private advantage_listAdvantage_containerAdvantageModifierSkill_bonus[] skill_bonusField;

        private string notesField;

        private byte versionField;

        private string enabledField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantageModifierCost cost
        {
            get
            {
                return this.costField;
            }
            set
            {
                this.costField = value;
            }
        }

        /// <remarks/>
        public byte levels
        {
            get
            {
                return this.levelsField;
            }
            set
            {
                this.levelsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool levelsSpecified
        {
            get
            {
                return this.levelsFieldSpecified;
            }
            set
            {
                this.levelsFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string affects
        {
            get
            {
                return this.affectsField;
            }
            set
            {
                this.affectsField = value;
            }
        }

        /// <remarks/>
        public string reference
        {
            get
            {
                return this.referenceField;
            }
            set
            {
                this.referenceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("skill_bonus")]
        public advantage_listAdvantage_containerAdvantageModifierSkill_bonus[] skill_bonus
        {
            get
            {
                return this.skill_bonusField;
            }
            set
            {
                this.skill_bonusField = value;
            }
        }

        /// <remarks/>
        public string notes
        {
            get
            {
                return this.notesField;
            }
            set
            {
                this.notesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string enabled
        {
            get
            {
                return this.enabledField;
            }
            set
            {
                this.enabledField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageModifierCost
    {

        private string typeField;

        private decimal valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public decimal Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageModifierSkill_bonus
    {

        private advantage_listAdvantage_containerAdvantageModifierSkill_bonusName nameField;

        private advantage_listAdvantage_containerAdvantageModifierSkill_bonusSpecialization specializationField;

        private byte amountField;

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantageModifierSkill_bonusName name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantageModifierSkill_bonusSpecialization specialization
        {
            get
            {
                return this.specializationField;
            }
            set
            {
                this.specializationField = value;
            }
        }

        /// <remarks/>
        public byte amount
        {
            get
            {
                return this.amountField;
            }
            set
            {
                this.amountField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageModifierSkill_bonusName
    {

        private string compareField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string compare
        {
            get
            {
                return this.compareField;
            }
            set
            {
                this.compareField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageModifierSkill_bonusSpecialization
    {

        private string compareField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string compare
        {
            get
            {
                return this.compareField;
            }
            set
            {
                this.compareField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantagePrereq_list
    {

        private advantage_listAdvantage_containerAdvantagePrereq_listAttribute_prereq attribute_prereqField;

        private advantage_listAdvantage_containerAdvantagePrereq_listSkill_prereq[] skill_prereqField;

        private advantage_listAdvantage_containerAdvantagePrereq_listAdvantage_prereq[] advantage_prereqField;

        private string allField;

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantagePrereq_listAttribute_prereq attribute_prereq
        {
            get
            {
                return this.attribute_prereqField;
            }
            set
            {
                this.attribute_prereqField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("skill_prereq")]
        public advantage_listAdvantage_containerAdvantagePrereq_listSkill_prereq[] skill_prereq
        {
            get
            {
                return this.skill_prereqField;
            }
            set
            {
                this.skill_prereqField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("advantage_prereq")]
        public advantage_listAdvantage_containerAdvantagePrereq_listAdvantage_prereq[] advantage_prereq
        {
            get
            {
                return this.advantage_prereqField;
            }
            set
            {
                this.advantage_prereqField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string all
        {
            get
            {
                return this.allField;
            }
            set
            {
                this.allField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantagePrereq_listAttribute_prereq
    {

        private string hasField;

        private string whichField;

        private string compareField;

        private byte valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string has
        {
            get
            {
                return this.hasField;
            }
            set
            {
                this.hasField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string which
        {
            get
            {
                return this.whichField;
            }
            set
            {
                this.whichField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string compare
        {
            get
            {
                return this.compareField;
            }
            set
            {
                this.compareField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public byte Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantagePrereq_listSkill_prereq
    {

        private advantage_listAdvantage_containerAdvantagePrereq_listSkill_prereqName nameField;

        private advantage_listAdvantage_containerAdvantagePrereq_listSkill_prereqSpecialization specializationField;

        private advantage_listAdvantage_containerAdvantagePrereq_listSkill_prereqLevel levelField;

        private string hasField;

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantagePrereq_listSkill_prereqName name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantagePrereq_listSkill_prereqSpecialization specialization
        {
            get
            {
                return this.specializationField;
            }
            set
            {
                this.specializationField = value;
            }
        }

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantagePrereq_listSkill_prereqLevel level
        {
            get
            {
                return this.levelField;
            }
            set
            {
                this.levelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string has
        {
            get
            {
                return this.hasField;
            }
            set
            {
                this.hasField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantagePrereq_listSkill_prereqName
    {

        private string compareField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string compare
        {
            get
            {
                return this.compareField;
            }
            set
            {
                this.compareField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantagePrereq_listSkill_prereqSpecialization
    {

        private string compareField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string compare
        {
            get
            {
                return this.compareField;
            }
            set
            {
                this.compareField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantagePrereq_listSkill_prereqLevel
    {

        private string compareField;

        private byte valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string compare
        {
            get
            {
                return this.compareField;
            }
            set
            {
                this.compareField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public byte Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantagePrereq_listAdvantage_prereq
    {

        private advantage_listAdvantage_containerAdvantagePrereq_listAdvantage_prereqName nameField;

        private advantage_listAdvantage_containerAdvantagePrereq_listAdvantage_prereqNotes notesField;

        private advantage_listAdvantage_containerAdvantagePrereq_listAdvantage_prereqLevel levelField;

        private string hasField;

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantagePrereq_listAdvantage_prereqName name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantagePrereq_listAdvantage_prereqNotes notes
        {
            get
            {
                return this.notesField;
            }
            set
            {
                this.notesField = value;
            }
        }

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantagePrereq_listAdvantage_prereqLevel level
        {
            get
            {
                return this.levelField;
            }
            set
            {
                this.levelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string has
        {
            get
            {
                return this.hasField;
            }
            set
            {
                this.hasField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantagePrereq_listAdvantage_prereqName
    {

        private string compareField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string compare
        {
            get
            {
                return this.compareField;
            }
            set
            {
                this.compareField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantagePrereq_listAdvantage_prereqNotes
    {

        private string compareField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string compare
        {
            get
            {
                return this.compareField;
            }
            set
            {
                this.compareField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantagePrereq_listAdvantage_prereqLevel
    {

        private string compareField;

        private byte valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string compare
        {
            get
            {
                return this.compareField;
            }
            set
            {
                this.compareField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public byte Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageSkill_bonus
    {

        private advantage_listAdvantage_containerAdvantageSkill_bonusName nameField;

        private advantage_listAdvantage_containerAdvantageSkill_bonusSpecialization specializationField;

        private advantage_listAdvantage_containerAdvantageSkill_bonusAmount amountField;

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantageSkill_bonusName name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantageSkill_bonusSpecialization specialization
        {
            get
            {
                return this.specializationField;
            }
            set
            {
                this.specializationField = value;
            }
        }

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantageSkill_bonusAmount amount
        {
            get
            {
                return this.amountField;
            }
            set
            {
                this.amountField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageSkill_bonusName
    {

        private string compareField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string compare
        {
            get
            {
                return this.compareField;
            }
            set
            {
                this.compareField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageSkill_bonusSpecialization
    {

        private string compareField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string compare
        {
            get
            {
                return this.compareField;
            }
            set
            {
                this.compareField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantageSkill_bonusAmount
    {

        private string per_levelField;

        private sbyte valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string per_level
        {
            get
            {
                return this.per_levelField;
            }
            set
            {
                this.per_levelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public sbyte Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
    public enum ItemsChoiceType2
    {

        /// <remarks/>
        attribute_bonus,

        /// <remarks/>
        base_points,

        /// <remarks/>
        categories,

        /// <remarks/>
        cr,

        /// <remarks/>
        dr_bonus,

        /// <remarks/>
        levels,

        /// <remarks/>
        melee_weapon,

        /// <remarks/>
        modifier,

        /// <remarks/>
        name,

        /// <remarks/>
        notes,

        /// <remarks/>
        points_per_level,

        /// <remarks/>
        prereq_list,

        /// <remarks/>
        reference,

        /// <remarks/>
        skill_bonus,

        /// <remarks/>
        type,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantage_container
    {

        private string nameField;

        private string referenceField;

        private string notesField;

        private advantage_listAdvantage_containerAdvantage_containerCategories categoriesField;

        private advantage_listAdvantage_containerAdvantage_containerAdvantage[] advantageField;

        private byte versionField;

        private string openField;

        private string typeField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string reference
        {
            get
            {
                return this.referenceField;
            }
            set
            {
                this.referenceField = value;
            }
        }

        /// <remarks/>
        public string notes
        {
            get
            {
                return this.notesField;
            }
            set
            {
                this.notesField = value;
            }
        }

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantage_containerCategories categories
        {
            get
            {
                return this.categoriesField;
            }
            set
            {
                this.categoriesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("advantage")]
        public advantage_listAdvantage_containerAdvantage_containerAdvantage[] advantage
        {
            get
            {
                return this.advantageField;
            }
            set
            {
                this.advantageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string open
        {
            get
            {
                return this.openField;
            }
            set
            {
                this.openField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantage_containerCategories
    {

        private string categoryField;

        /// <remarks/>
        public string category
        {
            get
            {
                return this.categoryField;
            }
            set
            {
                this.categoryField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantage_containerAdvantage
    {

        private object[] itemsField;

        private ItemsChoiceType1[] itemsElementNameField;

        private byte versionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("attribute_bonus", typeof(advantage_listAdvantage_containerAdvantage_containerAdvantageAttribute_bonus))]
        [System.Xml.Serialization.XmlElementAttribute("base_points", typeof(sbyte))]
        [System.Xml.Serialization.XmlElementAttribute("categories", typeof(advantage_listAdvantage_containerAdvantage_containerAdvantageCategories))]
        [System.Xml.Serialization.XmlElementAttribute("cr", typeof(advantage_listAdvantage_containerAdvantage_containerAdvantageCR))]
        [System.Xml.Serialization.XmlElementAttribute("levels", typeof(byte))]
        [System.Xml.Serialization.XmlElementAttribute("modifier", typeof(advantage_listAdvantage_containerAdvantage_containerAdvantageModifier))]
        [System.Xml.Serialization.XmlElementAttribute("name", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("notes", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("points_per_level", typeof(byte))]
        [System.Xml.Serialization.XmlElementAttribute("prereq_list", typeof(advantage_listAdvantage_containerAdvantage_containerAdvantagePrereq_list))]
        [System.Xml.Serialization.XmlElementAttribute("reference", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("skill_bonus", typeof(advantage_listAdvantage_containerAdvantage_containerAdvantageSkill_bonus))]
        [System.Xml.Serialization.XmlElementAttribute("type", typeof(string))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType1[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantage_containerAdvantageAttribute_bonus
    {

        private string attributeField;

        private advantage_listAdvantage_containerAdvantage_containerAdvantageAttribute_bonusAmount amountField;

        /// <remarks/>
        public string attribute
        {
            get
            {
                return this.attributeField;
            }
            set
            {
                this.attributeField = value;
            }
        }

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantage_containerAdvantageAttribute_bonusAmount amount
        {
            get
            {
                return this.amountField;
            }
            set
            {
                this.amountField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantage_containerAdvantageAttribute_bonusAmount
    {

        private string per_levelField;

        private byte valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string per_level
        {
            get
            {
                return this.per_levelField;
            }
            set
            {
                this.per_levelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public byte Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantage_containerAdvantageCategories
    {

        private string[] categoryField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("category")]
        public string[] category
        {
            get
            {
                return this.categoryField;
            }
            set
            {
                this.categoryField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantage_containerAdvantageCR
    {

        private string adjField;

        private byte valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string adj
        {
            get
            {
                return this.adjField;
            }
            set
            {
                this.adjField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public byte Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantage_containerAdvantageModifier
    {

        private object[] itemsField;

        private ItemsChoiceType[] itemsElementNameField;

        private byte versionField;

        private string enabledField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("affects", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("cost", typeof(advantage_listAdvantage_containerAdvantage_containerAdvantageModifierCost))]
        [System.Xml.Serialization.XmlElementAttribute("name", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("notes", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("reference", typeof(string))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string enabled
        {
            get
            {
                return this.enabledField;
            }
            set
            {
                this.enabledField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantage_containerAdvantageModifierCost
    {

        private string typeField;

        private decimal valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public decimal Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
    public enum ItemsChoiceType
    {

        /// <remarks/>
        affects,

        /// <remarks/>
        cost,

        /// <remarks/>
        name,

        /// <remarks/>
        notes,

        /// <remarks/>
        reference,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantage_containerAdvantagePrereq_list
    {

        private advantage_listAdvantage_containerAdvantage_containerAdvantagePrereq_listAdvantage_prereq[] advantage_prereqField;

        private string allField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("advantage_prereq")]
        public advantage_listAdvantage_containerAdvantage_containerAdvantagePrereq_listAdvantage_prereq[] advantage_prereq
        {
            get
            {
                return this.advantage_prereqField;
            }
            set
            {
                this.advantage_prereqField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string all
        {
            get
            {
                return this.allField;
            }
            set
            {
                this.allField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantage_containerAdvantagePrereq_listAdvantage_prereq
    {

        private advantage_listAdvantage_containerAdvantage_containerAdvantagePrereq_listAdvantage_prereqName nameField;

        private advantage_listAdvantage_containerAdvantage_containerAdvantagePrereq_listAdvantage_prereqNotes notesField;

        private string hasField;

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantage_containerAdvantagePrereq_listAdvantage_prereqName name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantage_containerAdvantagePrereq_listAdvantage_prereqNotes notes
        {
            get
            {
                return this.notesField;
            }
            set
            {
                this.notesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string has
        {
            get
            {
                return this.hasField;
            }
            set
            {
                this.hasField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantage_containerAdvantagePrereq_listAdvantage_prereqName
    {

        private string compareField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string compare
        {
            get
            {
                return this.compareField;
            }
            set
            {
                this.compareField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantage_containerAdvantagePrereq_listAdvantage_prereqNotes
    {

        private string compareField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string compare
        {
            get
            {
                return this.compareField;
            }
            set
            {
                this.compareField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantage_containerAdvantageSkill_bonus
    {

        private advantage_listAdvantage_containerAdvantage_containerAdvantageSkill_bonusName nameField;

        private advantage_listAdvantage_containerAdvantage_containerAdvantageSkill_bonusSpecialization specializationField;

        private sbyte amountField;

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantage_containerAdvantageSkill_bonusName name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public advantage_listAdvantage_containerAdvantage_containerAdvantageSkill_bonusSpecialization specialization
        {
            get
            {
                return this.specializationField;
            }
            set
            {
                this.specializationField = value;
            }
        }

        /// <remarks/>
        public sbyte amount
        {
            get
            {
                return this.amountField;
            }
            set
            {
                this.amountField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantage_containerAdvantageSkill_bonusName
    {

        private string compareField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string compare
        {
            get
            {
                return this.compareField;
            }
            set
            {
                this.compareField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class advantage_listAdvantage_containerAdvantage_containerAdvantageSkill_bonusSpecialization
    {

        private string compareField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string compare
        {
            get
            {
                return this.compareField;
            }
            set
            {
                this.compareField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
    public enum ItemsChoiceType1
    {

        /// <remarks/>
        attribute_bonus,

        /// <remarks/>
        base_points,

        /// <remarks/>
        categories,

        /// <remarks/>
        cr,

        /// <remarks/>
        levels,

        /// <remarks/>
        modifier,

        /// <remarks/>
        name,

        /// <remarks/>
        notes,

        /// <remarks/>
        points_per_level,

        /// <remarks/>
        prereq_list,

        /// <remarks/>
        reference,

        /// <remarks/>
        skill_bonus,

        /// <remarks/>
        type,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
    public enum ItemsChoiceType3
    {

        /// <remarks/>
        advantage,

        /// <remarks/>
        advantage_container,

        /// <remarks/>
        name,

        /// <remarks/>
        notes,

        /// <remarks/>
        reference,
    }
}