using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using GM.DataAccess.Entities;
using GM.DataAccess.Interfaces;
using GM.Models;
using GM.Models.Public;

namespace GM.DataAccess
{
    public class ChallengesRepository
    {
        public List<ChallengeCategory> GetChallengeCategories()
        {
            using (var context = new greenMoneyEntities())
            {
               var categories = context.ChallengeCategories
                   .OrderBy(c => c.Name);
                

                List<ChallengeCategory> list = new List<ChallengeCategory>();
                foreach (var category in categories)
                {
                    ChallengeCategory model = new ChallengeCategory();
                    Utils.CopyProperties(category, model);

                    list.Add(model);
                }

                return list;
            }
        }

        public List<ChallengeAwardPointsOption> GetChallengeAwardPointsOptions()
        {
            using (var context = new greenMoneyEntities())
            {
                var options = context.AwardPointsOptions
                    .OrderBy(c => c.Name);


                List<ChallengeAwardPointsOption> list = new List<ChallengeAwardPointsOption>();
                foreach (var option in options)
                {
                    ChallengeAwardPointsOption model = new ChallengeAwardPointsOption();
                    Utils.CopyProperties(option, model);

                    list.Add(model);
                }

                return list;
            }
        }

        public List<ChallengeParticipationFrequency> GetChallengeParticipationFrequency()
        {
            using (var context = new greenMoneyEntities())
            {
                var frequencies = context.ParticipationFrequency
                    .OrderBy(c => c.Name);


                List<ChallengeParticipationFrequency> list = new List<ChallengeParticipationFrequency>();
                foreach (var frequency in frequencies)
                {
                    ChallengeParticipationFrequency model = new ChallengeParticipationFrequency();
                    Utils.CopyProperties(frequency, model);

                    list.Add(model);
                }

                return list;
            }
        }


        //challenge
        public int? CreateChallenge(Guid providerUserKey, ChallengeModel model)
        {
            using (var context = new greenMoneyEntities())
            {
                var user = context.Users1.FirstOrDefault(u => u.Id == providerUserKey);
                if (user != null)
                {
                    Challenges challenge = new Challenges();

                    Utils.CopyProperties(model, challenge);

                    challenge.DateAdded = DateTime.Now;

                    context.Challenges.Add(challenge);
                     try
                    {
                        context.SaveChanges();

                        return challenge.Id;

                    }
                     catch (DbEntityValidationException dbEx)
                     {
                         foreach (var validationErrors in dbEx.EntityValidationErrors)
                         {
                             foreach (var validationError in validationErrors.ValidationErrors)
                             {
                                 Trace.TraceInformation("Class: {0}, Property: {1}, Error: {2}",
                                     validationErrors.Entry.Entity.GetType().FullName,
                                     validationError.PropertyName,
                                     validationError.ErrorMessage);
                             }
                         }

                         return null;
                     }
                }
            }
            return null;
        }
    }
}
