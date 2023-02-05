using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper mapper;
        private readonly DataContext context;

        public CharacterService(IMapper mapper, DataContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceReponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = mapper.Map<Character>(newCharacter);
            await context.Characters.AddAsync(character);
            await context.SaveChangesAsync();
            var dbCharacters = await context.Characters.ToListAsync();
            serviceReponse.Data= mapper.Map<List<GetCharacterDto>>(dbCharacters);
            return serviceReponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                var character = await context.Characters.FirstOrDefaultAsync(c => c.Id == id);
                if (character == null)
                    throw new Exception($"Character with Id '{id}' Not Found.");
                context.Characters.Remove(character);
                await context.SaveChangesAsync();
                var dbCharacters = await context.Characters.ToListAsync();
                serviceResponse.Data = mapper.Map<List<GetCharacterDto>>(dbCharacters);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceReponse = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await context.Characters.ToListAsync();
            serviceReponse.Data= mapper.Map<List<GetCharacterDto>>(dbCharacters);
            return serviceReponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceReponse = new ServiceResponse<GetCharacterDto>();
            var dbCharacter = await context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            if(dbCharacter == null)
            {
                throw new Exception("Character not found");
            }
            serviceReponse.Data = mapper.Map<GetCharacterDto>(dbCharacter);
            return serviceReponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                var character = await context.Characters.FirstOrDefaultAsync(c => c.Id == updateCharacter.Id);
                if (character == null)
                    throw new Exception($"Character with Id '{updateCharacter.Id}' Not Found.");
                character.Name = updateCharacter.Name;
                character.HitPoints = updateCharacter.HitPoints;
                character.Strength = updateCharacter.Strength;
                character.Defense = updateCharacter.Defense;
                character.Intelligence = updateCharacter.Intelligence;
                character.Class = updateCharacter.Class;
                await context.SaveChangesAsync();
                serviceResponse.Data = mapper.Map<GetCharacterDto>(character);
            }
            catch(Exception ex)
            {
                serviceResponse.Success= false;
                serviceResponse.Message=ex.Message;
            }
            return serviceResponse;
        }
    }
}
